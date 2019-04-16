using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Vision.V1;
using Grpc.Auth;
using System.Speech.Synthesis;

namespace OCR_CsharpAPP
{
    class MainFunctional
    {
        private static SpeechSynthesizer _sound= new SpeechSynthesizer
        {
            Volume = 100,
            Rate = 0
        };
        //สเตทของสถานะการอ่านคำ Play คือเล่นตั้งแต่ต้น , Pause คือ หยุดพักไว้ก่อน , Continue คือ เล่นต่อจากที่ Pause ไว้
        enum State { Play,Pause, Continue };
        //Flag เป็นตัวกำหนดสถานะการอ่านคำ
        private static State Flag = State.Play;
        //key ที่เป็นของ service account ใช้สำหรับการ authentication ในการใช้บริการ API OCR ของ Google Cloud Platform(GCP)
        private static string key = @"{
'type': 'service_account',
  'project_id': 'weighty-time-237311',
  'private_key_id': '675c76ba6379b6e6d71ae31fc9153b62ff01c6bf',
  'private_key': '-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDt7Y6+NUi/KW/Q\nrYM9nzZwbv1XUumrtYLhxs1JxIVoRZ56geFUCZWuPJXRS7muPBZzjpc5s5si2tPr\nvg0DjcWL5LOKNKD8GKARWCFfJygKjOvrSfa5JRGki7QOZvF7EcicdpvjPL3f1lAs\n+DC8ImAJTgckKQEQ57B34dOX0dF4MzIqJ3yra9vIBjBGeeSqBZ1PdxUTvpARlg/E\nM3FcMDUkfjA2XahjOSL+spkZacr1qYNBWya3TzpejmDj2+0llfq2COvcFTHCksrh\nkEf9OhYnGqfw+lgJc6WeYVkWDmSXL0I8Z/qzZ3lSHG4yex+d5m5JATX/LJCeUbvG\nR44kDe09AgMBAAECggEAOROq0V7zeWZdnEwvpnX+B4lcrZjmlQpZYEPChh6GmW3Q\nF5f2P+R/u5ltoFSf8dgY07ZieVVokzs3KP/B2/cj64wc9PvsnaVjqt1/PfYlASju\nxJZNAXvOyHA6LNVNu0YYH87FVUVCUIL3X15XxJ4jdz/4bxiGGLHYD95FFg/fxgtd\nkX1lJ5rzCxlFjxc6lVuu442zIIeNRGHtKjaa79fxoUvu6x/mVYbrrVvWrrZPvPIs\nsx7r8G28lEB3ZyauRqPS1GXlUFqRpLkbsBAtlTibQQ6k2ZR/0t6oQhOU+GyBwiF9\nl0fHqEdayDJ9Y4Rl9sSJzAy5QqPqutHxGGICvj34eQKBgQD/45LSGRjmTH7rKRBw\nMvU+jyB6ozgiH7BV3YIUGAkFGXGlmBcj92zYqJqF6HbCMxrHLb5v7GePaZb9FreA\nSDfNpweiqsRr5f1KX2eix/edwecYvOscxOYt3Pf+nLSWsfVuyK8rWFyMcqIZD8t+\ntP3QlmMt0vxlBY6h58udEhAv6QKBgQDuB/0h+eEYrYZbBaZsqPOFPKBWtLlqV+Rb\njNJTwOzCjy+YfIw+FDb9nh5LYjS/9jzaNC0xlAxTPmLvAiN6WqtazGthKmF7ye+H\nbyNc9x62fRDpCb3yjabz9KhESswtq7nXN5kwZwPCe6IWJu1d1BxmRsklDIjsgf3K\ngwYeX8SyNQKBgFsXhlKogIarQJsyORnX6hnlFajSpc7v/PP0MLQ6giuAjUZnyAUT\ntXnDO47j3DLxwlyZWiu5unwBGLDr/1L5YnQhO8SaieXnL63kHJ6EFQ/h0QSra/8H\n0PEOsnG/E0J6A2b2pfUGNZwBytalGsn2YEx63L+ZViQYQFg+jetSXNTRAoGAFlGX\ncZUdfxPeMjCwbyXUV5zcp+SgMhF1rwPgQMwpJwEIBHNBLxz0Hwmxa34U1h7/i7Iy\nvqBUG4YQ/rojm3he8s+SDfVMWLARjpBkL3ZYYeIOMNuh/Nk9W0iIcobU6D9e2Ig1\nC+3M03KGOy+BODgIRarDd37aY9q4ckGg1D5EkjUCgYA1upFTcp3cEsWk954nxKtT\n3fQdwuZq5LVi5pQETayndPnYIYp0PageLp/8gz1tVj0EjjKMYFn1BbEZxP4Rwj4p\n3y2iv9uvef/7NuKzb0TfBKUcF/cLEU9gizc+eqKicNuNYkAZennM0uI5KMLz4W5n\nXC5r6g/sR4zDPOaRvGJkZA==\n-----END PRIVATE KEY-----\n',
  'client_email': 'ocrc-428@weighty-time-237311.iam.gserviceaccount.com',
  'client_id': '113854285087095949866',
  'auth_uri': 'https://accounts.google.com/o/oauth2/auth',
  'token_uri': 'https://oauth2.googleapis.com/token',
  'auth_provider_x509_cert_url': 'https://www.googleapis.com/oauth2/v1/certs',
  'client_x509_cert_url': 'https://www.googleapis.com/robot/v1/metadata/x509/ocrc-428%40weighty-time-237311.iam.gserviceaccount.com'}";
        private static string Path = "";
        public static string ImagePath //Path ที่อยู่ของรูปภาพที่จะใช้สำหรับแปลงเป็น Text
        {
            get => Path;
            set => Path = value;
        }

        //ฟังก์ชั่นสำหรับการดึง Text ออกมาจากรูปภาพ
        public async static Task<string> Async_OCR_Operation()
        {
            ImageAnnotatorClient client;
            Grpc.Core.Channel channel;
            GoogleCredential credential;
            Image pic;
            try
            {
                //credential คือ การร้องขอการใช้บริการของ Google โดยที่รูปแบบการขอบริการจะเป็นในรูปแบบเชิงเกี่ยวกับการประมวลผลรูปภาพ
                credential = GoogleCredential.FromJson(key).CreateScoped(ImageAnnotatorClient.DefaultScopes);
                // channel คือ ช่องทางการเชื่อมต่อระหว่าง client และ server ของ google ในการร้องของใช้บริการ
                channel = new Grpc.Core.Channel(ImageAnnotatorClient.DefaultEndpoint.ToString(), credential.ToChannelCredentials());
                // Object Client จะทำการสร้างการเชื่อมต่อในการร้องขอบริการ ImageAnnotator โดยผ่านช่องทางที่มีรายละเอียดอยู่ใน channel
                client = ImageAnnotatorClient.Create(channel);
                // ทำการดึงรูปภาพจาก path ที่อยู่ใน ImagePath
                pic = Image.FromFile(ImagePath);
                // และใช้ client.DetectText ในการตรวจจับหาตัวอักษรหรือข้อความจากรูปภาพ pic
                var text1 = client.DetectText(pic);
                // text1 จะเป็น json ที่ได้รับมาจากการตรวจจับหาข้อความ โดยที่เราจะดึงในส่วนของ Description ที่เก็บเนื้อหาของข้อความในรูปภาพเท่านั้น
                return text1[0].Description;
            }
            catch (ArgumentException Ax) // catch Exception ที่ในกรณีที่รูปภาพนั้นไม่มีข้อความ
            {
                throw Ax; //throw กลับไปยังต้นทาง
            }
        }


        //ฟังก์ชั่นการอ่านข้อความ
        public static async void SpeakText()
        {
            //เลือกเสียงสำหรับการพูด ในที่นี้เป็นเสียงผู้ชาย
            _sound.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Child);
            switch(Flag) //เลือกสถานะในการอ่าน
            {
                case State.Play: //เริ่มอ่าน
                    Task<string> result = Async_OCR_Operation();
                    _sound.SpeakAsync(await result);
                    Flag = State.Pause;
                    break;
                case State.Pause: //หยุดพักการอ่าน
                    _sound.Pause();
                    Flag = State.Continue;
                    break;
                case State.Continue: //อ่านต่อจากการพัก
                    _sound.Resume();
                    Flag = State.Pause;
                    break;
            }
            // SpeakCompleted เป็น Event ที่เมื่อการอ่านข้อความนั้นจบลงแล้วจะคืนค่า Flag ให้เป็น State.Play เพื่อในกรณีที่ผู้ใช้ต้องการให้อ่านใหม่อีกรอบ
            _sound.SpeakCompleted+= delegate(object sender,SpeakCompletedEventArgs s){
                Flag = State.Play;
            };
        }

        //ฟังก์ชั่นการรีเซ็ตการอ่าน
        public static void Stopping()
        {
            //หยุดการอ่านทั้งหมด และ คืนค่าสถานะการอ่านเป็นสถานะเริ่มต้น
            _sound.SpeakAsyncCancelAll();
            Flag = State.Play;
        }
    }
}
