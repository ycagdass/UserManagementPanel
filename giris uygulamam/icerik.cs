using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; // SQL Server ile bağlantı için gerekli kütüphane
using System.IO;

namespace giris_uygulamam
{
    public partial class icerik : Form
    {
        public icerik()
        {
            InitializeComponent();
        }

        #region sql sorgu işlemleri
        void goster()
        {
            SqlConnection baglanti = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\Resources\\Database1.mdf;Integrated Security=True");
            baglanti.Open();
            SqlDataAdapter vericek = new SqlDataAdapter("select * from icerik icerik order by ad", baglanti);
            DataSet ds = new DataSet();
            vericek.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0]; // DataGridView'e verileri ata
            baglanti.Close(); // Veritabanı bağlantısını kapat
        }
        #endregion

        private void icerik_Load(object sender, EventArgs e)
        {
            goster(); // Form yüklendiğinde verileri göster
            dataGridView1.MultiSelect = false; // Çoklu seçim özelliğini kapat
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Satır bazında seçim yap

        }
        public string SeciliKayıtNo;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int SeciliKayıt = dataGridView1.SelectedCells[0].RowIndex;
            SeciliKayıtNo = dataGridView1.Rows[SeciliKayıt].Cells[0].Value.ToString();

            // TextBox'lara verileri ata
            textBox1.Text = dataGridView1.Rows[SeciliKayıt].Cells[1].Value?.ToString();
            textBox2.Text = dataGridView1.Rows[SeciliKayıt].Cells[2].Value?.ToString();
            textBox3.Text = dataGridView1.Rows[SeciliKayıt].Cells[3].Value?.ToString();
            textBox4.Text = dataGridView1.Rows[SeciliKayıt].Cells[4].Value?.ToString();
            textBox5.Text = dataGridView1.Rows[SeciliKayıt].Cells[5].Value?.ToString();

            // Resmi kontrol et ve yükle
            object imgObj = dataGridView1.Rows[SeciliKayıt].Cells[6].Value;

            if (imgObj != DBNull.Value && imgObj != null)
            {
                try
                {
                    byte[] imgData = (byte[])imgObj;
                    MemoryStream ms = new MemoryStream(imgData);
                    pictureBox1.Image = Image.FromStream(ms); // Resmi yükle
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Resim yüklenirken hata oluştu: " + ex.Message);
                    pictureBox1.Image = null;
                }
            }
            else
            {
                pictureBox1.Image = null; // Resim yoksa temizle
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            ekle frm = new ekle();
            frm.Show(); // Yeni formu aç
            this.Visible = false; // Mevcut formu gizle
        }
    }
}
