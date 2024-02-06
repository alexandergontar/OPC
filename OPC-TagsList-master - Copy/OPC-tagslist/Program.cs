/*=====================================================================
  File:      OPCCSharp.cs

  Summary:   OPC sample client for C#

-----------------------------------------------------------------------
  This file is part of the Viscom OPC Code Samples.

  Copyright(c) 2001 Viscom (www.viscomvisual.com) All rights reserved.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.
======================================================================*/

using System;
using System.Threading;
using System.Threading.Tasks;
using OPC.Common;
using OPC.Data.Interface;
using OPC.Data;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using OPC_tagslist;
using System.Collections.Generic;

namespace CSSample
{
    class Tester
    {
        private string tempData;
        private readonly Config confFromFile;

        public Tester() 
        { 
          tempData = String.Empty;
            try
            {
                TextReader textReader = File.OpenText("Config.txt");
                string config = textReader.ReadToEnd();
                textReader.Close();
                confFromFile = JsonConvert.DeserializeObject<Config>(config);
            }
            catch (Exception)
            {
                throw;
            }            
        }
        public void Run()
        {
            while (true)
            {
                Work();
            }
        }
        private void Work()
        {           
            List<Content> contents = new List<Content>();
            //Создаем экземпляр объекта OpcServerList
            OpcServerList ServersList = new OpcServerList();
            bool serverPresent = false;
            //Создаем массив OpcServers[]
            OpcServers[] SrvListArray;
            //формируем массив серверов
            ServersList.ListAllData20(out SrvListArray);
            Console.WriteLine("Доступные OPC сервера:");
            for (int i = 0; i < SrvListArray.Length; i++)
            {
                Console.Write("{0}  {1}", i + 1, SrvListArray[i]); // +1, т.к. индекс начинается с нуля
                if (SrvListArray[i].ProgID == confFromFile.Opc.Server)
                {
                    Console.WriteLine("\nСервер найден!");
                    serverPresent = true;
                }
                Console.WriteLine();
            }
            Console.WriteLine("----------");
            if (!serverPresent)
            {
                Console.WriteLine("Сервер не найден...");
                Console.ReadKey();
                return;
            }            
            try
            { 
                //Подключение к серверу
                OpcServer theSrv = new OpcServer();               
                string serverProgID = confFromFile.Opc.Server;                
                Console.WriteLine($"Connecting to: {serverProgID}");
                theSrv.Connect(serverProgID);
                Console.WriteLine("Формирую список...");
                Thread.Sleep(500); //задержка для подключения
                //создаем ArrayList, и помещаем в него все теги
                ArrayList tagList = new ArrayList();
                List<string> newTagList = new List<string>();
                theSrv.Browse(OPCBROWSETYPE.OPC_FLAT, out tagList);
                Console.WriteLine("Всего тегов: " + tagList.Count);
                //вывод всех тегов в коносль       
                foreach (object tag in tagList)
                {
                    Console.WriteLine(tag.ToString());
                }
                // вывод отобранных тегов и их значений
                Console.WriteLine("Отобранные теги со значениями: ");
                foreach (var item in confFromFile.Opc.tags)
                    foreach (object tag in tagList)
                    {                        
                        if (tag.ToString() == item)
                        {
                            OPCProperty[] props;
                            theSrv.QueryAvailableProperties(tag.ToString(), out props);
                            int[] propIDs = new int[props.Length];
                            for (int i = 0; i < props.Length; i++)
                            {
                                propIDs[i] = props[i].PropertyID;
                            }

                            OPCPropertyData[] data;
                            theSrv.GetItemProperties(tag.ToString(), propIDs, out data);                          
                            Content content = new Content();
                            content.TagName = tag.ToString();
                            List<TagData> tagDatas = new List<TagData>();
                            content.TagDatas = tagDatas;
                            for (int i = 0; i < data.Length; i++)
                            {                                
                                TagData tagData = new TagData();
                                tagData.Id = data[i].PropertyID;
                                tagData.Data = data[i].Data.ToString();
                                content.TagDatas.Add(tagData);                                
                            }
                            contents.Add(content);                            
                        }
                    }
                                
#pragma warning disable CS0168 // Variable is declared but never used
                try
                {  // Формируем JSON данные для запроса
                    Message message = new Message();                    
                    message.Header = "Всего объектов: " + contents.Count;                   
                    message.contents = contents;
                    string jsonData = JsonConvert.SerializeObject(message);
                    Console.WriteLine(jsonData);                   
                    // если данные совпадают с предыдущими, запрос не посылаем
                    if (tempData == jsonData)
                    {
                        Console.WriteLine("\n Данные без изменений, отправка не требуется.");
                        Console.ReadKey();                        
                        return;
                    }
                    // если данные изменились, запоминаем и посылаем http запрос
                    tempData = jsonData;
                    Console.WriteLine("Now post JSON to server, please wait...");
                    Client client = new Client();
                    Task<string> result = client.Send(jsonData, confFromFile.POST, confFromFile.Username, confFromFile.Password);
                    result.Wait(20000);
                    Console.WriteLine(result.Result.ToString());
                    Console.Write("Новые данные успешно отправлены.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }                
#pragma warning restore CS0168 // Variable is declared but never used
            }
            catch
            {
                Console.Write("Ошибка. Либо неправильный ввод, либо не удалось подключиться к серверу");
            }
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Tester tst = new Tester();            
            tst.Run();
        }
    }
}
