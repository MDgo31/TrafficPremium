using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Data.Sql;
using System.Data.SqlClient;

namespace WindowsFormsApplication22
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        decimal provincerate;
        decimal claimlevel;
        decimal vehiclepremium;
        XmlDocument vehicles = new XmlDocument();
        XmlDocument provinces = new XmlDocument();
        XmlDocument claimlevels = new XmlDocument();
        DateTime May = Convert.ToDateTime("31.05.2017");
        decimal increaserate = 1M;
        decimal final;
        decimal increase = 1.01M;
        DateTime selected;
        int repeat;
        


       
        private void label3_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Hasarsızlık Kademinizi öğrenmek için sbm.org.tr'den kontrol edebilirsiniz.", label3, label3.Width - 10, label3.Height - 10, 5000);
        }

        private void label1_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Trafik Poliçe Bitiş tarihini giriniz! Bu işlemi en sonda yapınız", label1, label1.Width - 50, label1.Height -50, 5000);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //we have use three different tables in order to calculate traffic premiums
           vehicles.Load("../../araç.xml");
            provinces.Load("../../ils.xml");
            claimlevels.Load("../../basamakr.xml");
            XmlNodeList vehicle = vehicles.DocumentElement.SelectNodes("Table");
            XmlNodeList province = provinces.DocumentElement.SelectNodes("Table");
            XmlNodeList level = claimlevels.DocumentElement.SelectNodes("Table");
            //we will fill combobox so end user can choose from them! In this section we will create vehicle combobox!
            foreach (XmlNode vec in vehicle)
            {
                string vehiclename = vec.SelectSingleNode("AracGrubu").InnerText;
                cmbArac.Items.Add(vehiclename);
            }
            //In section example we will create province combobox!!
            foreach (XmlNode pro in province)
            {
                string provincename = pro.SelectSingleNode("Il").InnerText;
                cmbiller.Items.Add(provincename);
            }
            foreach (XmlNode lev in level)
            {
                string levelname = lev.SelectSingleNode("BasamakNo").InnerText;
                cmbxHasarsızlık.Items.Add(levelname);
            }
          
        }
  
        private void cmbxHasarsızlık_SelectedIndexChanged(object sender, EventArgs e)
        {
            //after user selects claim level, related claim level be applied 
            try
            {
                if (cmbxHasarsızlık.SelectedIndex > -1)
                {

                    XmlNode claimlev = claimlevels.DocumentElement.SelectSingleNode(string.Format("Table[BasamakNo='{0}']", cmbxHasarsızlık.SelectedItem));
                    claimlevel = Convert.ToDecimal(claimlev.SelectSingleNode("Fiat").InnerText, System.Globalization.CultureInfo.GetCultureInfo("en"));
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void cmbiller_SelectedIndexChanged(object sender, EventArgs e)
        {
            // related province factor will be called after selection of the province by the user
            if (cmbiller.SelectedIndex > -1)
            {
                XmlNode pro = provinces.DocumentElement.SelectSingleNode(string.Format("Table[Il='{0}']", cmbiller.SelectedItem));
                provincerate = Convert.ToDecimal(pro.SelectSingleNode("Oran").InnerText, System.Globalization.CultureInfo.GetCultureInfo("en"));
            }

        }

        private void cmbArac_SelectedIndexChanged(object sender, EventArgs e)
        {//  finally  vehicle default premium will called after vehicle type selected! 
            if (cmbArac.SelectedIndex > -1)
            {

                XmlNode vec = vehicles.DocumentElement.SelectSingleNode(string.Format("Table[AracGrubu='{0}']", cmbArac.SelectedItem));
                vehiclepremium = Convert.ToDecimal(vec.SelectSingleNode("Fiyat").InnerText, System.Globalization.CultureInfo.GetCultureInfo("en"));
                //label5.Text = arac.SelectSingleNode("Fiyat").InnerText;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //After May , user will be charged additional 0,1% for per month!  
            //So we will check the date if it has passed May then we will apply increased rate otherwise it will be 1 
            selected = Convert.ToDateTime(dateTimePicker1.Value);
            repeat = Convert.ToInt32(selected.Month - May.Month)+((selected.Year-May.Year)*12);
            label5.Text = "";
            final = 1;
            decimal final2 = 1;
            increaserate = 1;

            if (repeat>0)
            {
                for (int i = 0; i < repeat; i++)
                {
                    increaserate *= increase;

                }
                final = increaserate * claimlevel * vehiclepremium * provincerate;
            }
            else
            {
                final = increaserate * claimlevel * vehiclepremium * provincerate;
            }
            final2 = final;//100;
            label5.Text = final2.ToString("c2");
         
        }
    }
}
