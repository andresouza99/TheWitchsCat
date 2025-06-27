using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoSenac
{
    public partial class FormVendedor : Form
    {
        public FormVendedor()
        {
            InitializeComponent();
            lblUser.Text = "Olá, " + Form1.form1.usuario.Text; //mensagem com o nome do usuario logado
        }

        private Form formAtual = null;
        private void abrirChildForm(Form childForm) //carrega as telas sem apagar o menu lateral
        {
            formAtual?.Close();
            formAtual = childForm;
            childForm.TopLevel = false;
            childForm.Dock = DockStyle.None;
            panelForm.Controls.Add(childForm);
            childForm.BringToFront();
            childForm.Show();
        }

        private void btnCadastarCliente_Click(object sender, EventArgs e)
        {
            abrirChildForm(new FormCliente());
        }

        private void btnRegistrarVenda_Click(object sender, EventArgs e)
        {
            abrirChildForm(new FormVenda());
        }

        private void btnEstoque_Click(object sender, EventArgs e)
        {
            abrirChildForm(new FormEstoque());
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
        #region Closing
        private void FormVendedor_FormClosing(object sender, FormClosingEventArgs e)
        {
            const string pergunta = "Tem certeza que quer fechar o programa?";
            var escolha = MessageBox.Show(pergunta, "Fechando...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (escolha == DialogResult.No) e.Cancel = true;
        }

        private void FormVendedor_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.ExitThread();
        }
        #endregion
    }
}
