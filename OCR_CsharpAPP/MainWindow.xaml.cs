using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace OCR_CsharpAPP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowImage.Source = null;
            Totext.IsEnabled = false; // Totext คือปุ่มสำหรับการแปลงรูปภาพให้เป็นข้อความ มีค่าเริ่มต้นให้กดไม่ได้หากไม่มีรูปภาพ
            Listen.IsEnabled = false; // Listen คือปุ่มสำหรับให้ระบบอ่านข้อความที่ถูกแปลงจากปุ่ม Totext แล้ว โดยมีค่าเริ่มต้นให้กดไม่ได้หากไม่มีรุปภาพ
            Stop.IsEnabled = false; // Stop คือปุ่มสำหรับให้ระบบหยุดอ่านข้อความและเซ็ตให้การอ่านครั้งต่อไปเป็นการอ่านใหม่ โดยมีค่าเริ่มต้นให้กดไม่ได้หากไม่มีรูปภาพ
        }

        //ฟังก์ชั่นสำหรับเปิด Dialog ในการค้นหารูปภาพเพื่อนำมาแปลงเป็นข้อความ
        private void Findpic(object sender, RoutedEventArgs e)
        {
            //Object ของหน้าต่าง Dialog ในการค้นหารูปภาพ
            OpenFileDialog Openfile = new OpenFileDialog
            {
                Title = "Find a Picture to Translate", //Title คือชื่อหน้าต่าง
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" + //Filter ในที่นี้คือหาเฉพาะรูปภาพเท่านั้น
        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
        "Portable Network Graphic (*.png)|*.png"
            };
            if (Openfile.ShowDialog() == true)
            {
                ShowImage.Source = new BitmapImage(new Uri(Openfile.FileName)); //หากเลือกรูปภาพแล้วจะถูกนำไปโชว์
                MainFunctional.ImagePath = Openfile.FileName; //เซ็ตพาทรูปภาพให้กับ ImagePath เพื่อนำไปเก็บไว้สำหรับแปลงเป็นข้อความ
                Totext.IsEnabled = true; //ปุ่ม Totext สามารถใช้งานได้ เนื่องจากมีรูปภาพแล้ว
                Listen.IsEnabled = false; //ปุ่ม Listen ไม่สามารถใช้งานได้หากยังไม่ทำการแปลงข้อความด้วยปุ่ม Totext
                Stop.IsEnabled = false; //ปุ่ม Stop ไม่สามารถใช้งานได้หากยังไม่ทำการแปลงข้อความด้วยปุ่ม Totext
            }
        }

        private async void ShowText(object sender,RoutedEventArgs e)
        {
            try
            {
                Task<string> result_text = MainFunctional.Async_OCR_Operation(); //แปลงรูปภาพเป็นข้อความ
                showtext.Text = await result_text; //showtext คือพื้นที่ในการแสดงข้อความจากการแปลงรูปภาพเป็นข้อความ
                Totext.IsEnabled = false; //ปุ่ม Totext จะไม่สามารถใช้ได้เนื่องจากได้ทำการแปลงรูปภาพเป็นข้อความไปแล้ว
                Listen.IsEnabled = true; //ปุ่ม Listen สามารถใช้งานได้เนื่องจากมีข้อความแล้ว
                Stop.IsEnabled = true; //ปุ่ม Stop สามารถใช้งานได้เนื่องจากมีข้อความแล้ว
            }
            catch(ArgumentException Ax) //จับ Exception จากการที่รูปภาพที่นำมาแปลงนั้นไม่มีข้อความ
            {
                MessageBox.Show("There is no text in this Image to Convert"); //แสดงข้อความแจ้งเตือนว่ารูปภาพที่ใช้นั้นไม่มีข้อความ
            }
        }

        private void Text_to_speech(object sender,RoutedEventArgs e) //เรียกให้ระบบอ่านข้อความ
        {
            MainFunctional.SpeakText();
        }

        private void StopSpeak(object sender,RoutedEventArgs e) //เรียกให้ระบบหยุดอ่านข้อความพร้อมกับการอ่านครั้งต่อไปจะเป็นการอ่านใหม่ทั้งหมด
        {
            MainFunctional.Stopping();
        }

        
    }
}
