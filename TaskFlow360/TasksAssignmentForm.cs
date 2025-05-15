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
    public partial class TasksAssignmentForm : Form
    {
        private int cagriId;
        private string baslik;
        private string yoneticiId;

        // Constructor with three parameters to fix CS1729  
        public TasksAssignmentForm(int cagriId, string baslik, string yoneticiId)
        {
            InitializeComponent();
            this.cagriId = cagriId;
            this.baslik = baslik;
            this.yoneticiId = yoneticiId;
        }
    }
}
