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
using denemeVT;

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
        public int SeciliKayıt;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SeciliKayıt = dataGridView1.SelectedCells[0].RowIndex;
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

        private void button2_Click(object sender, EventArgs e)
        {
            int seciliKayıt = dataGridView1.SelectedCells[0].RowIndex;
            SeciliKayıtNo = dataGridView1.Rows[seciliKayıt].Cells[0].Value.ToString();
            DialogResult onay =MessageBox.Show(SeciliKayıtNo + " Numaralı Kayıt Silinsin Mi?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (onay == DialogResult.Yes)
            {
                SqlConnection baglanti = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\Resources\\Database1.mdf;Integrated Security=True");
                baglanti.Open();
                SqlCommand cmd1 = new SqlCommand("DELETE FROM icerik WHERE ID=@kimlik", baglanti);
                cmd1.Parameters.AddWithValue("@kimlik", dataGridView1.Rows[seciliKayıt].Cells[0].Value); 
                cmd1.ExecuteNonQuery(); // Sorguyu çalıştır 
                MessageBox.Show("Kayıt Başarıyla Silindi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                baglanti.Close(); // Bağlantıyı kapat
                goster(); // Verileri tekrar göster
                pictureBox1.Image = null; // Resmi temizle
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();

            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Önce seçili bir kayıt olup olmadığını kontrol et
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen güncellenecek bir kaydı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Güncelleme işlemine devam etmeden metodu sonlandır
            }

            // Seçili kayıt varsa, güncelleme formunu aç
            update frm = new update();

            // TextBox'lara verileri aktar
            frm.textBox1.Text = textBox1.Text;
            frm.textBox2.Text = textBox2.Text;
            frm.textBox3.Text = textBox3.Text;
            frm.textBox4.Text = textBox4.Text;
            frm.textBox5.Text = textBox5.Text;

            // Resmi aktar
            frm.pictureBox1.Image = pictureBox1.Image;

            // Duzenlenecek_ID'yi ayarla
            Program.Duzenlenecek_ID = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();

            // Güncelleme formunu göster ve mevcut formu kapat
            frm.Show();
            this.Close();
        }
    }
}
