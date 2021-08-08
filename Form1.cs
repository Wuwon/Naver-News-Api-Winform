using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Selenium
{
    public partial class Form1 : Form
    {
        const string _apiUrl = IgnoreValues.apiUrl;
        const string _ClientId = IgnoreValues.ClientId;
        const string _ClientSecret = IgnoreValues.ClientSecret;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            data_Parsing();
        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            string results = getResult();
            string text = listView1.FocusedItem.SubItems[0].Text;
            var i = Convert.ToInt32(text);
            var parseJson = JObject.Parse(results);
            var link = parseJson["items"][(i - 1)]["link"].ToString();
            Process.Start("chrome.exe", link);
        }
        
        private string getResult()
        {

            string keyword = keyword_textbox.Text;
            
            string display = comboBox1.Text;
            string sort = "sim";
            if (radioButton2.Checked == true)
            {
                sort = "date";
            }

            string query = string.Format("?query={0}&display={1}&sort={2}", keyword, display, sort);

            WebRequest request = WebRequest.Create(_apiUrl + query);
            request.Headers.Add("X-Naver-Client-Id", _ClientId);
            request.Headers.Add("X-Naver-Client-Secret", _ClientSecret);

            string requestResult = "";
            using(var response = request.GetResponse())
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    using(var reader = new StreamReader(dataStream))
                    {
                        requestResult = reader.ReadToEnd();
                    }
                }
            }
            return requestResult;
        }
        private void data_Parsing()
        {
            try
            {
                string results = getResult();
                results = results.Replace("<b>", "");
                results = results.Replace("</b>", "");

                var parseJson = JObject.Parse(results);
                var countsOfDisplay = Convert.ToInt32(parseJson["display"]);
                var countsOfResults = Convert.ToInt32(parseJson["total"]);
                listView1.Items.Clear();
                for (int i = 0; i < countsOfDisplay; i++)
                {
                    ListViewItem item = new ListViewItem((i + 1).ToString());
                    var title = parseJson["items"][i]["title"].ToString();
                    title = title.Replace("&quot;", "\"");

                    var date = parseJson["items"][i]["pubDate"].ToString();
                    date = date.Replace("Jan", "1")
                                 .Replace("Feb", "2")
                                 .Replace("Mar", "3")
                                 .Replace("Apr", "4")
                                 .Replace("May", "5")
                                 .Replace("Jun", "6")
                                 .Replace("Jul", "7")
                                 .Replace("Aug", "8")
                                 .Replace("Sep", "9")
                                 .Replace("Oct", "10")
                                 .Replace("Nov", "11")
                                 .Replace("Dec", "12");
                    string[] split_date;
                    split_date = date.Split(' ');

                    date = split_date[3] + "." + split_date[2] + "." + split_date[1];

                    item.SubItems.Add(title);
                    item.SubItems.Add(date);
                    listView1.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }


    }
}
