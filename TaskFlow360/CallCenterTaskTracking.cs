using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskFlow360
{
    public partial class CallCenterTaskTracking : Form
    {
        public CallCenterTaskTracking()
        {
            InitializeComponent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void CallCenterTaskTracking_Load(object sender, EventArgs e)
        {
            OrnekVerileriYukle();
        }

        private void OrnekVerileriYukle()
        {
            // Bekleyen çağrılar için örnek veriler
            CagrilarDGV.Rows.Add("#2458", "Rapor oluşturma sorunu", "Teknik", "Yüksek", "Atandı", "14/04/2022");
            CagrilarDGV.Rows.Add("#2459", "Yeni müşteri kaydı hatası", "Yazılım", "Orta","Bekliyor", "04/04/2022");
            CagrilarDGV.Rows.Add("#2460", "Mail sistemi bağlantı sorunu", "Altyapı", "Normal","Tamamlandı", "03/04/2022");
            CagrilarDGV.Rows.Add("#2461", "Fatura oluşturma problemi", "Destek", "Orta", "Atandı", "20/04/2022");
            CagrilarDGV.Rows.Add("#2458", "Rapor oluşturma sorunu", "Teknik", "Yüksek", "Atandı", "14/04/2022");
            CagrilarDGV.Rows.Add("#2459", "Yeni müşteri kaydı hatası", "Yazılım", "Orta", "Tamamlandı", "04/04/2022");
            CagrilarDGV.Rows.Add("#2460", "Mail sistemi bağlantı sorunu", "Altyapı", "Normal", "Bekliyor", "03/04/2022");
            CagrilarDGV.Rows.Add("#2461", "Fatura oluşturma problemi", "Destek", "Orta", "Bekliyor", "20/04/2022");
            CagrilarDGV.Rows.Add("#2458", "Rapor oluşturma sorunu", "Teknik", "Yüksek", "Bekliyor", "14/04/2022");
            CagrilarDGV.Rows.Add("#2459", "Yeni müşteri kaydı hatası", "Yazılım", "Orta", "Atandı", "04/04/2022");

            // oncelik hücrelerine renk verme
            foreach (DataGridViewRow row in CagrilarDGV.Rows)
            {
                string oncelik = row.Cells["oncelik"].Value.ToString();
                if (oncelik == "Yüksek")
                    row.Cells["oncelik"].Style.BackColor = ColorTranslator.FromHtml("#f85c5c");
                else if (oncelik == "Orta")
                    row.Cells["oncelik"].Style.BackColor = ColorTranslator.FromHtml("#f0ad4e");
                else
                    row.Cells["oncelik"].Style.BackColor = ColorTranslator.FromHtml("#63c966");

                row.Cells["oncelik"].Style.ForeColor = Color.White;
                row.Cells["oncelik"].Style.Font = new Font("Arial", 10, FontStyle.Bold);
                row.Cells["ataButon"].Style.BackColor = ColorTranslator.FromHtml("#5d4e9d");
                row.Cells["ataButon"].Style.ForeColor = Color.White;
            }
        }

        private void CagrilarDGV_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTest = CagrilarDGV.HitTest(e.X, e.Y);

                if (hitTest.Type == DataGridViewHitTestType.Cell && hitTest.RowIndex >= 0)
                {
                    CagrilarDGV.ClearSelection();
                    CagrilarDGV.Rows[hitTest.RowIndex].Selected = true;
                    CagrilarDGV.CurrentCell = CagrilarDGV.Rows[hitTest.RowIndex].Cells[hitTest.ColumnIndex];

                    contextMenuStrip1.Show(CagrilarDGV, e.Location);
                }
                else
                {
                    contextMenuStrip1.Hide();
                }
            }
        }


    }
}

