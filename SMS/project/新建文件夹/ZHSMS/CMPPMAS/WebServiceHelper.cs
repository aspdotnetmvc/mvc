﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Net;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Web.Services.Description;
using System.Web.Services.Protocols;

namespace CMPPMAS
{
    /* 调用方式
     *   string url = "http://www.webservicex.net/globalweather.asmx" ;
     *   *   string[] args = new string[2] ; 
     *   *   args[0] = "Hangzhou";
     *   *   args[1] = "China" ; 
     *   *   object result = WebServiceHelper.InvokeWebService(url ,"GetWeather" ,args) ;
     *   *   Response.Write(result.ToString()); 
     *   */
    public class WebServiceHelper
    {
        #region InvokeWebService
        /// < summary>
        ///  动态调用web服务
        ///< /summary> 
        /// 
        /// < param name="url">WSDL服务地址< /param>
        /// < param name="methodname">方法名< /param>
        ///  < param name="args">参数< /param>
        ///  < returns>< /returns>
        public object InvokeWebService(string url, string methodname, object[] args)
        {
            return this.InvokeWebService(url, null, methodname, args);
        }

        /// < summary>
        /// 动态调用web服务
        /// < /summary>
        /// < param name="url">WSDL服务地址< /param>
        /// < param name="classname">类名< /param>
        /// < param name="methodname">方法名< /param> 
        /// < param name="args">参数< /param> 
        /// < returns>< /returns> 
        public object InvokeWebService(string url, string classname, string methodname, object[] args)
        {
            //string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";
            string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";
            if ((classname == null) || (classname == ""))
            {
                classname = WebServiceHelper.GetWsClassName(url);
            }
            try
            {
                //获取WSDL 
                WebClient wc = new WebClient();
                if (!url.ToUpper().Contains("WSDL"))
                {
                    url = string.Format("{0}?{1}", url, "WSDL");
                }
                Stream stream = wc.OpenRead(url);
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);
                //生成客户端代理类代码
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider icc = new CSharpCodeProvider();
                //设定编译参数
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");
                //编译代理类 
                CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }
                //生成代理实例，并调用方法  
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(@namespace + "." + classname, true, true);
                object obj = Activator.CreateInstance(t);
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);
                return mi.Invoke(obj, args);
                /*        
                 * PropertyInfo propertyInfo = type.GetProperty(propertyname);     
                 * return propertyInfo.GetValue(obj, null);      
                 * */
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
            }
        }

        private static string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');
            if (pps[0].Contains("?"))
            {
                return pps[0].Split('?')[0];
            }
            return pps[0];
        }
        #endregion
    }
}
