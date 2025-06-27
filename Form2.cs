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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            lblUser.Text = "Olá, " + Form1.form1.usuario.Text; //mensagem com o nome do usuario logado
        }

        private void EsconderMenu()
        {
            if (panelCadastar.Visible == true) panelCadastar.Visible = false;
            if (panelRegistrar.Visible == true) panelRegistrar.Visible = false;
        }

        private void MostrarMenu(Panel painel)
        {
            if (painel.Visible == false)
            {
                EsconderMenu();
                painel.Visible = true;
            }
            else painel.Visible = false;
        }

        #region Closing
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            const string pergunta = "Tem certeza que quer fechar o programa?";
            var escolha = MessageBox.Show(pergunta, "Fechando...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (escolha == DialogResult.No) e.Cancel = true;
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.ExitThread();
        }
        #endregion

        private void btnCadastar_Click(object sender, EventArgs e)
        {
            MostrarMenu(panelCadastar);
        }
        private void btnProduto_Click(object sender, EventArgs e)
        {
            abrirChildForm(new FormProduto());
        }

        private void btnCliente_Click(object sender, EventArgs e)
        {
            abrirChildForm(new FormCliente());
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            MostrarMenu(panelRegistrar);
        }

        private void btnCompra_Click(object sender, EventArgs e)
        {
            abrirChildForm(new FormCompra());
        }

        private void btnVenda_Click(object sender, EventArgs e)
        {
            abrirChildForm(new FormVenda());
        }

        private void btnEstoque_Click(object sender, EventArgs e)
        {
            EsconderMenu();
            abrirChildForm(new FormEstoque());
        }

        private void btnRelatorio_Click(object sender, EventArgs e)
        {
            EsconderMenu();
            abrirChildForm(new FormRelatorio());
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
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

        private void btnFuncionario_Click(object sender, EventArgs e)
        {
            abrirChildForm(new FormFuncionario());
        }
    }
}
