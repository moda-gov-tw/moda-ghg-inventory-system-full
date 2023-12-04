using Project.Common;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace Project.Common
{
    public class MailHelper
    {
        /// <summary>
        /// 是否開啟
        /// </summary>
        public bool IsOpen { get; set; } = true;
        /// <summary>
        /// 發送人名稱
        /// </summary>
        public string FromName { get; set; } = "";
        /// <summary>
        /// 發送人信箱
        /// </summary>
        public string FromEmail { get; set; } = "";
        /// <summary>
        /// 發送人密碼
        /// </summary>
        public string FromPwd { get; set; } = "";
        /// <summary>
        /// 發送協議
        /// </summary>
        public string Smtp { get; set; } = "";
        /// <summary>
        /// 協議端口
        /// </summary>
        public int Port { get; set; } = 587;
        /// <summary>
        /// 郵件簽名
        /// </summary>
        public string Signature { get; set; }
        /// <summary>
        /// 是否使用SSL協議
        /// </summary>
        public bool UseSsl { get; set; } = false;

        public MailHelper()
        {
            IsOpen = AppSetting.GetValue<bool>("EmailSet:IsOpen");
            FromName = AppSetting.GetValue("EmailSet:FromName");
            FromEmail = AppSetting.GetValue("EmailSet:FromEmail");
            Smtp = AppSetting.GetValue("EmailSet:Smtp");
            FromPwd = AppSetting.GetValue("EmailSet:FromPwd");
            Port = AppSetting.GetValue<int>("EmailSet:Port");
            UseSsl = AppSetting.GetValue<bool>("EmailSet:UseSsl");
        }

        public MailHelper(string fromName, string fromEmail, string fromPwd, string smtp, int port, bool isSsl)
        {
            FromName = fromName;
            FromEmail = fromEmail;
            Smtp = smtp;
            FromPwd = fromPwd;
            Port = port;
            UseSsl = isSsl;
        }

        /// <summary>
        /// 發送一個
        /// </summary>
        /// <param name="toAddress"></param>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        /// <param name="path"></param>
        /// <param name="html"></param>
        public void SendMail(string toAddress, string subject, string text, string path = "", string html = "")
        {
            // LogHelper.WriteInfo($"send email:{toAddress} subject：{subject} text：{text} path：{path} html：{html} IsOpen：{IsOpen}");
            if (!IsOpen)
            {
                return;
            }
            if (string.IsNullOrEmpty(toAddress))
            {
                return;
            }
            IEnumerable<MailboxAddress> mailboxes = new List<MailboxAddress>() {
                new MailboxAddress(toAddress, toAddress)
            };

            SendMail(mailboxes, subject, text, path, html);
        }

        /// <summary>
        /// 發送多個信箱
        /// </summary>
        /// <param name="toAddress"></param>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        /// <param name="path"></param>
        /// <param name="html"></param>
        public void SendMail(string[] toAddress, string subject, string text, string path = "", string html = "")
        {
            // LogHelper.WriteInfo($"send email:{string.Join(',', toAddress)} subject：{subject} text：{text} path：{path} html：{html} IsOpen：{IsOpen}");
            if (!IsOpen)
            {
                return;
            }
            IList<MailboxAddress> mailboxes = new List<MailboxAddress>() { };
            foreach (var item in toAddress)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    mailboxes.Add(new MailboxAddress(item, item));
                }
            }

            SendMail(mailboxes, subject, text, path, html);
        }

        /// <summary>
        /// 發送郵件
        /// </summary>
        /// <param name="toAddress"></param>
        /// <param name="subject">主題</param>
        /// <param name="text"></param>
        /// <param name="path">附件url地址</param>
        /// <param name="html">網頁HTML內容</param>
        private void SendMail(IEnumerable<MailboxAddress> toAddress, string subject, string text, string path = "", string html = "")
        {
            try
            {
                MimeMessage message = new MimeMessage();
                //發件人
                message.From.Add(new MailboxAddress(FromName, FromEmail));
                //收件人
                message.To.AddRange(toAddress);
                message.Subject = subject;
                message.Date = DateTime.Now;

                //創建附件Multipart
                Multipart multipart = new Multipart("mixed");
                var alternative = new MultipartAlternative();
                //html內容
                if (!string.IsNullOrEmpty(html))
                {
                    var Html = new TextPart(TextFormat.Html)
                    {
                        Text = html
                    };
                    alternative.Add(Html);
                }
                //文本內容
                //if (!string.IsNullOrEmpty(text))
                //{
                var plain = new TextPart(TextFormat.Html)
                {
                    Text = text + "\r\n\n\n" + Signature
                };
                alternative.Add(plain);
                //}

                //附件
                if (!string.IsNullOrEmpty(path))
                {
                    string[] files = path.Split(",");
                    foreach (var file in files)
                    {
                        MimePart attachment = new()
                        {
                            Content = new MimeContent(File.OpenRead(file), ContentEncoding.Default),
                            //讀取文件，只能用絕對路徑
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            //文件名字
                            FileName = Path.GetFileName(path)
                        };
                        alternative.Add(attachment);
                    }
                }
                multipart.Add(alternative);
                //賦值郵件內容
                message.Body = multipart;

                //開始發送
                using var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                //Smtp伺服器
                //client.Connect("smtp.qq.com", 587, false);
                client.Connect(Smtp, Port, true);
                //登入，發送
                //特別說明，對於伺服器端的中文相應，Exception中有編碼問題，顯示亂碼了
                client.Authenticate(FromEmail, FromPwd);

                client.Send(message);
                //斷開
                client.Disconnect(true);
                Console.WriteLine($"發送郵件成功{DateTime.Now}");
            }
            catch (Exception ex)
            {
                // LogHelper.WriteError(ex, "發送郵件異常");
            }
        }
    }
}