using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;



namespace SerialPort
{
    public partial class From1 : Form
    {
        public From1()
        {
            InitializeComponent();
        }
             
        private void Form1_Load(object sender, EventArgs e)
        {
            //获取电脑上可用的串口名数组
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            comboBox_1.Items.AddRange(ports);//将Items表示comboBox1中得项列表,AddRange表示向项列表里添加数组
            //获取当前对象comboBox1中的项数，如果有选项即有端口则返回0
            comboBox_1.SelectedIndex = comboBox_1.Items.Count > 0 ? 0 : -1;
            //(.Text表示与控件相关联的文本)/显示窗体部件的默认设置;
            comboBox2.Text = "38400";
            comboBox3.Text = "1";//默停止位设为1；
            comboBox4.Text = "8";//数据位设为8；
            comboBox5.Text = "无";//校验位设为无

        }
        //打开串口按钮
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "打开串口")
            {
                try
                {
                    serialPort1.PortName = comboBox_1.Text;//获取在coboBox1中打开的串口号
                    //comboBox2获取选择的波特率，int.Parse(string s)表示将数字的字符串表示为等效的32位整数
                    serialPort1.BaudRate = int.Parse(comboBox2.Text);
                    serialPort1.DataBits = int.Parse(comboBox4.Text);//获取数据位
                    /*设置停止位*/
                    if (comboBox3.Text == "1") { serialPort1.StopBits = StopBits.One; }
                    else if (comboBox3.Text == "1.5") { serialPort1.StopBits = StopBits.OnePointFive; }
                    else if (comboBox3.Text == "2") { serialPort1.StopBits = StopBits.Two; }
                    /*设置奇偶校验*/
                    if (comboBox5.Text == "无") { serialPort1.Parity = Parity.None; }
                    else if (comboBox5.Text == "奇校验") { serialPort1.Parity = Parity.Odd; }
                    else if (comboBox5.Text == "偶校验") { serialPort1.Parity = Parity.Even; }
                    //打开串口
                    serialPort1.Open();
                    button1.Text = "关闭串口";//按钮提示关闭
                }
                //Exception表示在执行过程中发生的错误
                catch (Exception err)
                {
                    //MessageBox是个窗体用于显示，同时阻止其他程序的其他操作直至用户自行关闭
                    //.Show用于显示指定的文本消息
                    //string Exception.ToString---返回当前错误的字符串形式，“提示！”显示在错误框顶部
                    MessageBox.Show("打开失败：  " + err.ToString(), "提示!");
                }
            }
            else //关闭串口
            {
                try
                {
                    //关闭串口，将System.IO.Ports.serialPort.IsOpen属性设置为flase,并释放Stream对象
                    serialPort1.Close();
                }
                catch (Exception) { }
                button1.Text = "打开串口";
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            
            int Len = serialPort1.BytesToRead;//获取接收缓冲区的字节数
            byte[] buff = new byte[Len];//创建数组放置接受的字节
            //从缓冲区中读取从0-Len长度的字节数放入数组
            serialPort1.Read(buff, 0, Len);
            //将接收的字节重编码成字符串
            string str = Encoding.Default.GetString(buff);

            Invoke((new Action(() => //C# 3.0以后代替委托的新方法
            {
                if (checkBox1.Checked)//选中16进制勾选框
                {
                    textBox1.AppendText(byteToHexStr(buff));//对话框追加显示数据
                }
                else 
                {
                    textBox1.AppendText(str);
                }
            })));
        }
        //将接收到的字符数组转变为16进制字符串
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            try
            {
                if (bytes != null)
                {
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        //X为16进制,2表示每次输出2位数
                        returnStr += bytes[i].ToString("X2");
                        returnStr += " ";
                    }
                }
                return returnStr;
            }
            catch (Exception)
            {
                return returnStr;
            }
        }
        //清除显示按钮
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }
        //发送数据
        private void button3_Click(object sender, EventArgs e)
        {
            String Str = textBox2.Text.ToString();
            try
            {
                if (Str.Length > 0)
                {
                    if (checkBox2.Checked)
                    {
                        byte[] byt = strToToHexByte(Str);
                        serialPort1.Write(byt, 0, byt.Length);
                    }
                    else
                    {
                        serialPort1.Write(Str);
                    }
                }

            }
            catch (Exception) { }

        }
        //将发送字符串转变为16进制
        private static byte[] strToToHexByte(String hexString)
        {
            int i;
            hexString = hexString.Replace(" ", "");//清除空格
            if ((hexString.Length % 2) != 0)//奇数个
            {
                byte[] returnBytes = new byte[(hexString.Length + 1) / 2];
                try
                {
                    for (i = 0; i < (hexString.Length - 1) / 2; i++)
                    {
                        returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                    }
                    returnBytes[returnBytes.Length - 1] = Convert.ToByte(hexString.Substring(hexString.Length - 1, 1).PadLeft(2, '0'), 16);
                }
                catch
                {
                    MessageBox.Show("含有非16进制字符", "提示");
                    return null;
                }
                return returnBytes;
            }
            else
            {
                byte[] returnBytes = new byte[(hexString.Length) / 2];
                try
                {
                    for (i = 0; i < returnBytes.Length; i++)
                    {
                        returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                    }
                }
                catch
                {
                    MessageBox.Show("含有非16进制字符", "提示");
                    return null;
                }
                return returnBytes;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
        }
    }

}
