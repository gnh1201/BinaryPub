using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace SocialOnTheFile.Helper
{
    public static class FileCompany
    {
        static public string Read(string filePath)
        {
            // 회사 정보 추출
            string[] companies = new string[] {
                GetCompanyInfo(filePath),
                GetProductName(filePath),
                GetCopyrightInfo(filePath),
                GetOrganization(filePath)
            };
            foreach (string company in companies)
            {
                if (company != null && !company.Equals(string.Empty))
                {
                    return company;
                }
            }

            return "Unknown";
        }

        public static string GetOrganization(string filePath)
        {
            string organization = string.Empty;

            // 서명된 파일인 경우 인증서 정보 추출
            X509Certificate2 certificate = GetCertificateInfo(filePath);
            if (certificate != null)
            {
                // Subject 필드에서 O (Organization) 값을 찾아 회사 정보 추출
                string[] fields = certificate.Subject.Split(',');

                foreach (string field in fields)
                {
                    string[] keyValue = field.Trim().Split('=');

                    if (keyValue.Length == 2 && keyValue[0].Trim().Equals("O", StringComparison.OrdinalIgnoreCase))
                    {
                        organization = keyValue[1].Trim();
                    }
                }
            }

            return organization;
        }

        public static string GetCompanyInfo(string filePath)
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(filePath);
            return versionInfo.CompanyName;
        }

        public static string GetProductName(string filePath)
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(filePath);
            return versionInfo.ProductName;
        }

        public static string GetCopyrightInfo(string filePath)
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(filePath);
            return versionInfo.LegalCopyright;
        }

        static X509Certificate2 GetCertificateInfo(string filePath)
        {
            // GetCertificateInfo 구현
            try
            {
                // 파일에 디지털 서명이 있는지 확인
                Assembly assembly = Assembly.LoadFile(filePath);
                X509Certificate2 certificate = new X509Certificate2(assembly.Location);

                // 서명이 유효한지 확인 (옵션)
                X509Chain chain = new X509Chain();
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck; // 여러 인증서를 사용하는 경우 인증서 연쇄를 무시할 수 있습니다.

                if (chain.Build(certificate))
                {
                    return certificate;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
