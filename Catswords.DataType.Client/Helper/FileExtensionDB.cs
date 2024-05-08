﻿using Catswords.DataType.Client.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Catswords.DataType.Client.Helper
{
    class FileExtensionDB
    {
        public List<Indicator> Indicators;

        public FileExtensionDB()
        {
            Indicators = new List<Indicator>();
        }

        public void Fetch(string q)
        {
            try
            {
                bool isConnected = NetworkInterface.GetIsNetworkAvailable();
                if (isConnected == false)
                {
                    MessageBox.Show("인터넷에 연결되어 있지 않습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                    // 원격 주소에서 XML 다운로드
                string url = Config.SEARCH_URL + q;
                WebClient client = new WebClient();

                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
                client.Encoding = Encoding.UTF8;
                string xmlString = client.DownloadString(url);

                // XmlDocument 객체 생성
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString);

                // 루트 노드 가져오기
                XmlNodeList itemList = xmlDoc.SelectNodes("/claw/list/item");

                // 각 아이템을 반복하며 정보 출력
                foreach (XmlNode itemNode in itemList)
                {
                    // organization
                    Indicators.Add(new Indicator()
                    {
                        Id = itemNode.SelectSingleNode("id").InnerText,
                        CreatedAt = GetDateTimeFromString(itemNode.SelectSingleNode("datetime").InnerText),
                        Content = "This format published by " + itemNode.SelectSingleNode("organization").InnerText,
                        Url = ""
                    });

                    // description
                    Indicators.Add(new Indicator()
                    {
                        Id = itemNode.SelectSingleNode("id").InnerText,
                        CreatedAt = GetDateTimeFromString(itemNode.SelectSingleNode("datetime").InnerText),
                        Content = itemNode.SelectSingleNode("description").InnerText,
                        Url = ""
                    });

                    // content
                    Indicators.Add(new Indicator()
                    {
                        Id = itemNode.SelectSingleNode("id").InnerText,
                        CreatedAt = GetDateTimeFromString(itemNode.SelectSingleNode("datetime").InnerText),
                        Content = itemNode.SelectSingleNode("content").InnerText,
                        Url = ""
                    });

                    // openwith
                    Indicators.Add(new Indicator()
                    {
                        Id = itemNode.SelectSingleNode("id").InnerText,
                        CreatedAt = GetDateTimeFromString(itemNode.SelectSingleNode("datetime").InnerText),
                        Content = "Open with " + itemNode.SelectSingleNode("openwith").InnerText,
                        Url = ""
                    });

                    // first reported
                    Indicators.Add(new Indicator()
                    {
                        Id = itemNode.SelectSingleNode("id").InnerText,
                        CreatedAt = GetDateTimeFromString(itemNode.SelectSingleNode("datetime").InnerText),
                        Content = "This format first seen on " + GetDateTimeFromString(itemNode.SelectSingleNode("datetime").InnerText).ToString(),
                        Url = ""
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public static DateTime GetDateTimeFromString(string dateString)
        {
            DateTime parsedDateTime;

            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out parsedDateTime))
            {
                return parsedDateTime;
            }
            else
            {
                parsedDateTime = DateTime.Now;
            }

            return parsedDateTime;
        }
    }
}


