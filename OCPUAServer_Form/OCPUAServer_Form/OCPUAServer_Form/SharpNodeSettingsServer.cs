﻿/* ========================================================================
 * Copyright (c) 2005-2016 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Opc.Ua;
using Opc.Ua.Server;
using System.Security.Cryptography.X509Certificates;

namespace OCPUAServer
{
    public partial class SharpNodeSettingsServer : StandardServer
    {
        //override實作

        /// <summary>
        /// 建立一個主要的Node管理器給SERVER
        /// </summary>
        /// <param name="server"></param>
        /// <param name="configuration"></param>
        /// <returns>主節點管理器</returns>
        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            Console.WriteLine("Creating the Node Managers.");
            //Utils.Trace("Creating the Node Managers.");

            List<INodeManager> nodeManagers = new List<INodeManager>();

            // 創建自訂義節點管理器.
            nodeManagers.Add(new EmptyNodeManager(server, configuration));

            // 以此創建主節點.
            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }
        /// <summary>
        /// 加載應用程序的不可配置properties。
        /// </summary>
        /// <returns>應用程序屬性</returns>
        protected override ServerProperties LoadServerProperties()
        {
            ServerProperties properties = new ServerProperties();

            properties.ManufacturerName = "Yi";
            properties.ProductName = "SharpNodeSettingsServer";
            properties.ProductUri = "http://opcfoundation.org/Quickstart/ReferenceServer/v1.03";
            properties.SoftwareVersion = Utils.GetAssemblySoftwareVersion();
            properties.BuildNumber = Utils.GetAssemblyBuildNumber();
            properties.BuildDate = Utils.GetAssemblyTimestamp();

            return properties;
        }
        /// <summary>
        /// 建立資源管理給SERVER
        /// </summary>
        /// <param name="server"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        protected override ResourceManager CreateResourceManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            ResourceManager resourceManager = new ResourceManager(server, configuration);

            System.Reflection.FieldInfo[] fields = typeof(StatusCodes).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            foreach (System.Reflection.FieldInfo field in fields)
            {
                uint? id = field.GetValue(typeof(StatusCodes)) as uint?;

                if (id != null)
                {
                    resourceManager.Add(id.Value, "en-US", field.Name);
                }
            }
            return resourceManager;
        }
        /// <summary>
        /// 在SERVER入前呼叫
        /// </summary>
        /// <param name="configuration"></param>
        protected override void OnServerStarting(ApplicationConfiguration configuration)
        {
            Utils.Trace("The server is starting.");

            base.OnServerStarting(configuration);
        }
        /// <summary>
        /// 在SERVER載入後呼叫
        /// </summary>
        /// <param name="server"></param>
        protected override void OnServerStarted(IServerInternal server)
        {
            base.OnServerStarted(server);

            // request notifications when the user identity is changed. all valid users are accepted by default.
            server.SessionManager.ImpersonateUser += new ImpersonateEventHandler(SessionManager_ImpersonateUser);
        }
        /// <summary>
        /// 在SERVER關閉前清除
        /// </summary>
        protected override void OnServerStopping()
        {
            Debug.WriteLine("The Server is stopping.");

            base.OnServerStopping();

#if INCLUDE_Sample
            CleanSampleModel();
#endif
        }
        //================================================================

        #region User Validation Functions
        /// <summary>
        /// 在使用者需調換身分時調用
        /// </summary>
        private void SessionManager_ImpersonateUser(Session session, ImpersonateEventArgs args)
        {
            // check for a user name token.
            UserNameIdentityToken userNameToken = args.NewIdentity as UserNameIdentityToken;

            if (userNameToken != null)
            {
                args.Identity = VerifyPassword(userNameToken);
                return;
            }

            // check for x509 user token.
            X509IdentityToken x509Token = args.NewIdentity as X509IdentityToken;

            if (x509Token != null)
            {
                VerifyUserTokenCertificate(x509Token.Certificate);
                args.Identity = new UserIdentity(x509Token);
                //Utils.Trace("X509 Token Accepted: {0}", args.Identity.DisplayName);
                return;
            }
        }

        /// <summary>
        /// 驗證使用者名稱對應密碼
        /// </summary>
        private IUserIdentity VerifyPassword(UserNameIdentityToken userNameToken)
        {
            var userName = userNameToken.UserName;
            var password = userNameToken.DecryptedPassword;
            if (String.IsNullOrEmpty(userName))
            {
                // 空使用者名稱不認可
                throw ServiceResultException.Create(StatusCodes.BadIdentityTokenInvalid,
                    "Security token is not a valid username token. An empty username is not accepted.");
            }
            if (String.IsNullOrEmpty(password))
            {
                //空密碼不認可
                throw ServiceResultException.Create(StatusCodes.BadIdentityTokenRejected,
                    "Security token is not a valid username token. An empty password is not accepted.");
            }
            // 具有設定服務器權限的用戶
            if (userName == "sysadmin" && password == "demo")
            {
                return new SystemConfigurationIdentity(new UserIdentity(userNameToken));
            }
            // CTT驗證的標準用戶
            if (!((userName == "user1" && password == "password") ||
                (userName == "user2" && password == "password1")))
            {
                // 建構轉換:對應預設
                TranslationInfo info = new TranslationInfo(
                    "InvalidPassword",
                    "en-US",
                    "Invalid username or password.",
                    userName);

                // 建構異常:供應商定義的子代碼
                throw new ServiceResultException(new ServiceResult(
                    StatusCodes.BadUserAccessDenied,
                    "InvalidPassword",
                    LoadServerProperties().ProductUri,
                    new LocalizedText(info)));
            }
            return new UserIdentity(userNameToken);
        }
        /// <summary>
        /// 驗證使用者證書是否沒問題
        /// </summary>
        private void VerifyUserTokenCertificate(X509Certificate2 certificate)
        {
            try
            {
                CertificateValidator.Validate(certificate);

                // 確定是否自我簽名
                bool isSelfSigned = Utils.CompareDistinguishedName(certificate.Subject, certificate.Issuer);

                // 不允許自我簽名證書作為用戶許可
                if (isSelfSigned && Utils.HasApplicationURN(certificate))
                {
                    throw new ServiceResultException(StatusCodes.BadCertificateUseNotAllowed);
                }
            }
            catch (Exception e)
            {
                TranslationInfo info;// 建構轉換:對應預設
                StatusCode result = StatusCodes.BadIdentityTokenRejected;
                ServiceResultException se = e as ServiceResultException;
                if (se != null && se.StatusCode == StatusCodes.BadCertificateUseNotAllowed)
                {
                    info = new TranslationInfo(
                        "InvalidCertificate",
                        "en-US",
                        "'{0}' is an invalid user certificate.",
                        certificate.Subject);

                    result = StatusCodes.BadIdentityTokenInvalid;
                }
                else
                {
                    // 預設建構轉換
                    info = new TranslationInfo(
                        "UntrustedCertificate",
                        "en-US",
                        "'{0}' is not a trusted user certificate.",
                        certificate.Subject);
                }

                // create an exception with a vendor defined sub-code.建立異常:供應商子代碼
                throw new ServiceResultException(new ServiceResult(
                    result,
                    info.Key,
                    LoadServerProperties().ProductUri,
                    new LocalizedText(info)));
            }
        }
        #endregion

    }
}
