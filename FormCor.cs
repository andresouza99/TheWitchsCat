using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace ProjetoSenac
{
    public partial class FormCor : Form
    {
        public FormCor()
        {
            InitializeComponent();
            txtId.Text = ProximoId().ToString(); //preenche o campo codigo com o proximo id disponivel
            CarregarDados();
        }

        //chamando a variavel com o endereço da conexão
        private static string connectionString = Program.connectionString;
        string codigo = FormProduto.produto.codigo.Text; //codigo do produto

        private void CarregarDados() //metodo q preenche o gridview com os dados do bd
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    string query = "USE Projeto_G4 SELECT id, cor FROM cor_produto WHERE id_produto = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", codigo);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoResizeColumn(0);
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool Checar_Vazio() //checa se um dos campos obrigatorios esta vazio
        {
            if (txtCor.Text == "")
            {
                MessageBox.Show("Por favor digite uma cor", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            else return false;
        }

        private int ProximoId() //preenche o campo codigo com o proximo id disponivel
        {
            try
            {
                int i = 1;
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    while (i != 0) //verifica de 1 em 1 se o id esta disponivel
                    {
                        string query = "USE Projeto_G4 SELECT id, cor FROM cor_produto WHERE id = @id AND id_produto = @id_produto";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", i);
                            cmd.Parameters.AddWithValue("@id_produto", codigo);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dataGridView1.DataSource = dt;
                            dataGridView1.AutoResizeColumn(0);
                            if (dataGridView1.RowCount.ToString() != "0") i++; //se id esta em uso, continua o loop
                            else return i; //retorna id disponivel
                        }
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return 1;
        }

        private void LimparCampos() //limpa todas txtbox
        {
            txtCor.Clear();
            txtId.Clear();
        }

        private string CorUnica() //garante q a cor inserida seja unica
        {
            try
            {
                string id = "";
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 SELECT id, cor FROM cor_produto WHERE cor = @cor AND id_produto = @id_produto";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@cor", txtCor.Text);
                        cmd.Parameters.AddWithValue("@id_produto", codigo);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoResizeColumn(0);
                        if (dataGridView1.RowCount.ToString() != "0") //se cor já existe no bd, retorna o id dessa cor
                        {
                            id = dataGridView1.Rows[0].Cells[0].Value.ToString();
                            return id;
                        }
                        return "0"; //se cor nao existe no bd, retorna 0
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "";
        }

        private Boolean IdUnico() //impossibilita insert com id q ja esta em uso
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 SELECT id, cor FROM cor_produto WHERE id = @id AND id_produto = @id_produto";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtId.Text);
                        cmd.Parameters.AddWithValue("@id_produto", codigo);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoResizeColumn(0);
                        if (dataGridView1.RowCount.ToString() == "1") //se id ja esta em uso, retorna false
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        private void btnAdicionar_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text.ToString(), out _)) //verifica se codigo é numero inteiro
            {
                MessageBox.Show("O Código deve ser um número inteiro", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Checar_Vazio()) return; //se o campo estiver vazio, não faz o insert
            if (CorUnica() == "0" && IdUnico())
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                    {
                        conn.Open();
                        string query = "USE Projeto_G4 INSERT INTO cor_produto (id, id_produto, cor) VALUES (@id, @id_produto, @cor)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", txtId.Text);
                            cmd.Parameters.AddWithValue("@cor", txtCor.Text);
                            cmd.Parameters.AddWithValue("@id_produto", codigo);
                            cmd.ExecuteNonQuery();
                        }
                        if (!Form1.form1.admin)
                        {
                            string queryrelatorio = "USE Projeto_G4 INSERT INTO relatorio (id_funcionario, acao, descricao, datahora) VALUES (@id_funcionario, @acao, @descricao, @datahora)";
                            using (SqlCommand cmd = new SqlCommand(queryrelatorio, conn))
                            {
                                cmd.Parameters.AddWithValue("@id_funcionario", Form1.form1.user);
                                cmd.Parameters.AddWithValue("@acao", "Cadastro");
                                cmd.Parameters.AddWithValue("@descricao", "Cor: " + txtId.Text);
                                cmd.Parameters.AddWithValue("@datahora", DateTime.Now);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex) //caso der erro, mostra a mensagem de erro
                {
                    MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                LimparCampos(); //limpa todas txtbox
                txtId.Text = ProximoId().ToString(); //preenche o campo codigo com o proximo id disponivel
            }
            else if (!IdUnico()) MessageBox.Show("O código já está em uso", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else MessageBox.Show("Essa cor já está cadastrada", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            CarregarDados(); //preenche o gridview com os dados do bd
        }

        private void bntAlterar_Click(object sender, EventArgs e)
        {
            if (txtId.Text == "")
            {
                MessageBox.Show("Nenhuma cor selecionada", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtId.Text.ToString(), out _)) //verifica se codigo é numero inteiro
            {
                MessageBox.Show("O Código deve ser um número inteiro", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Checar_Vazio()) return;//se o campo estiver vazio, não faz o update
            if (IdUnico()) //precisa de um id existente para fazer o update
            {
                MessageBox.Show("Código inválido", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CarregarDados(); //preenche o gridview com os dados do bd
                return;
            }
            string cor = CorUnica();
            if (cor == "0" || cor == txtId.Text) //so permite o update se a cor for nova ou igual a atual
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                    {
                        conn.Open();
                        string query = "USE Projeto_G4 UPDATE cor_produto SET cor = @cor WHERE id = @id AND id_produto = @id_produto";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", txtId.Text);
                            cmd.Parameters.AddWithValue("@id_produto", codigo);
                            cmd.Parameters.AddWithValue("@cor", txtCor.Text);
                            cmd.ExecuteNonQuery();
                        }
                        if (!Form1.form1.admin)
                        {
                            string queryrelatorio = "USE Projeto_G4 INSERT INTO relatorio (id_funcionario, acao, descricao, datahora) VALUES (@id_funcionario, @acao, @descricao, @datahora)";
                            using (SqlCommand cmd = new SqlCommand(queryrelatorio, conn))
                            {
                                cmd.Parameters.AddWithValue("@id_funcionario", Form1.form1.user);
                                cmd.Parameters.AddWithValue("@acao", "Atualização");
                                cmd.Parameters.AddWithValue("@descricao", "Cor: " + txtId.Text);
                                cmd.Parameters.AddWithValue("@datahora", DateTime.Now);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex) //caso der erro, mostra a mensagem de erro
                {
                    MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                LimparCampos(); //limpa todas txtbox
            }
            else MessageBox.Show("Essa cor já está cadastrada", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            CarregarDados(); //preenche o gridview com os dados do bd
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int linha = Int32.Parse(e.RowIndex.ToString()); //pega numero da linha selecionada
                if (linha >= 0) //garante q linha seleciona seja maior q zero
                {
                    //preenche as txtbox com os valores da linha selecionada no gridview
                    txtId.Text = dataGridView1.Rows[linha].Cells[0].Value.ToString();
                    txtCor.Text = dataGridView1.Rows[linha].Cells[1].Value.ToString();
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCor_TextChanged(object sender, EventArgs e)
        {
            //se o campo cor tiver algum texto, torna visivel o botão com "x"
            if (txtCor.Text == "") btnX.Visible = false;
            else btnX.Visible = true;
        }

        private void btnX_Click(object sender, EventArgs e)
        {
            txtCor.Text = "";  //limpa o campo cor
        }

        private void btnX2_Click(object sender, EventArgs e)
        {
            txtId.Text = "";  //limpa o campo codigo
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //se o campo codigo tiver algum texto, torna visivel o botão com "x"
            if (txtId.Text == "") btnX2.Visible = false;
            else btnX2.Visible = true;
        }

        private void btnDeletar_Click(object sender, EventArgs e)
        {
            if (txtId.Text == "")
            {
                MessageBox.Show("Nenhuma cor selecionada", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtId.Text.ToString(), out _)) //verifica se codigo é numero inteiro
            {
                MessageBox.Show("O Código deve ser um número inteiro", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (IdUnico()) //precisa de um id valido no campo codigo
            {
                MessageBox.Show("Código inválido", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CarregarDados(); //preenche o gridview com os dados do bd
                return;
            }
            //linhas abaixo confirmam se a exclusao deve ser efetuada
            const string pergunta = "Tem certeza que deseja deletar?";
            var escolha = MessageBox.Show(pergunta, "Deletando...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (escolha == DialogResult.No)
            {
                CarregarDados(); //preenche o gridview com os dados do bd
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open(); 
                    string querycompra = "USE Projeto_G4 DELETE FROM item_compra WHERE id_cor = @id AND id_produto = @id_produto";
                    using (SqlCommand cmd = new SqlCommand(querycompra, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtId.Text);
                        cmd.Parameters.AddWithValue("@id_produto", codigo);
                        cmd.ExecuteNonQuery();
                    }
                    string queryvenda = "USE Projeto_G4 DELETE FROM item_venda WHERE id_cor = @id AND id_produto = @id_produto";
                    using (SqlCommand cmd = new SqlCommand(queryvenda, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtId.Text);
                        cmd.Parameters.AddWithValue("@id_produto", codigo);
                        cmd.ExecuteNonQuery();
                    }
                    string query = "USE Projeto_G4 DELETE FROM Estoque WHERE id_cor = @id AND id_produto = @id_produto";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtId.Text);
                        cmd.Parameters.AddWithValue("@id_produto", codigo);
                        cmd.ExecuteNonQuery();
                    }
                    string query2 = "USE Projeto_G4 DELETE FROM cor_produto WHERE id = @id AND id_produto = @id_produto";
                    using (SqlCommand cmd = new SqlCommand(query2, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtId.Text);
                        cmd.Parameters.AddWithValue("@id_produto", codigo);
                        cmd.ExecuteNonQuery();
                    }
                    if (!Form1.form1.admin)
                    {
                        string queryrelatorio = "USE Projeto_G4 INSERT INTO relatorio (id_funcionario, acao, descricao, datahora) VALUES (@id_funcionario, @acao, @descricao, @datahora)";
                        using (SqlCommand cmd = new SqlCommand(queryrelatorio, conn))
                        {
                            cmd.Parameters.AddWithValue("@id_funcionario", Form1.form1.user);
                            cmd.Parameters.AddWithValue("@acao", "Exclusão");
                            cmd.Parameters.AddWithValue("@descricao", "Cor: " + txtId.Text);
                            cmd.Parameters.AddWithValue("@datahora", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LimparCampos(); //limpa todas txtbox
            CarregarDados(); //preenche o gridview com os dados do bd
        }

        private void FormCor_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormProduto.produto.CarregarCores();
        }
    }
}
