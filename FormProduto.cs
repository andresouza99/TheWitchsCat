using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoSenac
{
    public partial class FormProduto : Form
    {
        public static FormProduto produto;
        public TextBox codigo;

        public FormProduto()
        {
            InitializeComponent();
            txtCodigo.Text = ProximoId().ToString(); //preenche o campo codigo com o proximo id disponivel
            CarregarDados(); //preenche o gridview com os dados do bd
            produto = this;
            codigo = txtCodigo;
        }

        //chamando a variavel com o endereço da conexão
        private readonly string connectionString = Program.connectionString;

        public void CarregarCores() //metodo q preenche o gridview com os dados do bd
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    string query = "USE Projeto_G4 SELECT cor FROM cor_produto WHERE id_produto = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView2.DataSource = dt;
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarDados() //metodo q preenche o gridview com os dados do bd
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    string query = "USE Projeto_G4 SELECT * FROM Produtos";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoResizeColumn(0);
                    }
                    string query2 = "USE Projeto_G4 SELECT cor FROM cor_produto WHERE id_produto = @id";
                    using (SqlCommand cmd = new SqlCommand(query2, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView2.DataSource = dt;
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                        string query = "USE Projeto_G4 SELECT * FROM Produtos WHERE id = @id";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", i);
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
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return 1;
        }

        private void LimparCampos() //limpa todas txtbox
        {
            txtCodigo.Clear();
            txtMarca.Clear();
            txtTipo.Clear();
            txtPreco.Clear();
            txtDescricao.Clear();
            txtPreco.Text = "R$ ";
        }

        private Boolean IdUnico() //impossibilita insert com id q ja esta em uso
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 SELECT * FROM Produtos WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
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

        private bool Checar_Vazio() //checa se um dos campos obrigatorios esta vazio
        {
            if (txtMarca.Text == "" || txtTipo.Text == "" || txtDescricao.Text == "" || txtPreco.Text == "")
            {
                MessageBox.Show("Um ou mais campos estão em branco", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            else if (ConvertePreco(txtPreco.Text) == "R$ 0,00")
            {
                MessageBox.Show("Preço inválido", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            else return false;
        }

        private string ConvertePreco(string preco)
        {
            int i = 0;
            string preco2 = "";
            bool cont = false, embranco = true;
            foreach (char c in preco)
            {
                if (char.IsDigit(c))
                {
                    if (i == 2) break;
                    preco2 += c;
                    embranco = false;
                    if (cont) i++;
                }
                else if (c.Equals('.') || c.Equals(','))
                {
                    preco2 += ",";
                    cont = true;
                }
                else continue;
            }
            if (embranco) return "R$ 0,00";
            if (i == 0) return "R$ " + preco2 + ",00";
            else if (i == 1) return "R$ " + preco2 + "0";
            else return "R$ " + preco2;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCodigo.Text.ToString(), out _)) //verifica se codigo é numero inteiro
            {
                MessageBox.Show("O Código deve ser um número inteiro", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Checar_Vazio()) return;//se um campo estiver vazio, não faz o insert
            //so faz o insert se id ainda nao existir no bd
            if (IdUnico())
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                    {
                        conn.Open();
                        string query = "USE Projeto_G4 INSERT INTO Produtos (id, marca, tipo, descricao, preco) VALUES (@id, @marca, @tipo, @descricao, @preco)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                            cmd.Parameters.AddWithValue("@marca", txtMarca.Text);
                            cmd.Parameters.AddWithValue("@tipo", txtTipo.Text);
                            cmd.Parameters.AddWithValue("@descricao", txtDescricao.Text);
                            cmd.Parameters.AddWithValue("@preco", ConvertePreco(txtPreco.Text));
                            cmd.ExecuteNonQuery();
                        }
                        if (!Form1.form1.admin)
                        {
                            string queryrelatorio = "USE Projeto_G4 INSERT INTO relatorio (id_funcionario, acao, descricao, datahora) VALUES (@id_funcionario, @acao, @descricao, @datahora)";
                            using (SqlCommand cmd = new SqlCommand(queryrelatorio, conn))
                            {
                                cmd.Parameters.AddWithValue("@id_funcionario", Form1.form1.user);
                                cmd.Parameters.AddWithValue("@acao", "Cadastro");
                                cmd.Parameters.AddWithValue("@descricao", "Produto: " + txtCodigo.Text);
                                cmd.Parameters.AddWithValue("@datahora", DateTime.Now);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    CarregarDados(); //preenche o gridview com os dados do bd
                }
                catch (Exception ex) //caso der erro, mostra a mensagem de erro
                {
                    MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                LimparCampos();  //limpa todas txtbox
                txtCodigo.Text = ProximoId().ToString(); //preenche o campo codigo com o proximo id disponivel
            }
            else MessageBox.Show("O código já está em uso", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            CarregarDados(); //preenche o gridview com os dados do bd
        }

        private void bntAlterar_Click(object sender, EventArgs e)
        {
            if (txtCodigo.Text == "")
            {
                MessageBox.Show("Nenhum produto selecionado", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtCodigo.Text.ToString(), out _)) //verifica se codigo é numero inteiro
            {
                MessageBox.Show("O Código deve ser um número inteiro", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Checar_Vazio()) return; //se um campo estiver vazio, não faz o update
            if (IdUnico()) //precisa de um id existente para fazer o update
            {
                MessageBox.Show("Código inválido", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CarregarDados(); //preenche o gridview com os dados do bd
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 UPDATE Produtos SET marca = @marca, tipo = @tipo, descricao = @descricao, preco = @preco WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@marca", txtMarca.Text);
                        cmd.Parameters.AddWithValue("@tipo", txtTipo.Text);
                        cmd.Parameters.AddWithValue("@descricao", txtDescricao.Text);
                        cmd.Parameters.AddWithValue("@preco", ConvertePreco(txtPreco.Text));
                        cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                        cmd.ExecuteNonQuery();
                    }
                    if (!Form1.form1.admin)
                    {
                        string queryrelatorio = "USE Projeto_G4 INSERT INTO relatorio (id_funcionario, acao, descricao, datahora) VALUES (@id_funcionario, @acao, @descricao, @datahora)";
                        using (SqlCommand cmd = new SqlCommand(queryrelatorio, conn))
                        {
                            cmd.Parameters.AddWithValue("@id_funcionario", Form1.form1.user);
                            cmd.Parameters.AddWithValue("@acao", "Atualização");
                            cmd.Parameters.AddWithValue("@descricao", "Produto: " + txtCodigo.Text);
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
            CarregarDados(); //preenche o gridview com os dados do bd
        }

        private void btnDeletar_Click(object sender, EventArgs e)
        {
            if (txtCodigo.Text == "")
            {
                MessageBox.Show("Nenhum produto selecionado", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtCodigo.Text.ToString(), out _)) //verifica se codigo é numero inteiro
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
                    string querycompra = "USE Projeto_G4 DELETE FROM item_compra WHERE id_produto = @id";
                    using (SqlCommand cmd = new SqlCommand(querycompra, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                        cmd.ExecuteNonQuery();
                    }
                    string queryvenda = "USE Projeto_G4 DELETE FROM item_venda WHERE id_produto = @id";
                    using (SqlCommand cmd = new SqlCommand(queryvenda, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                        cmd.ExecuteNonQuery();
                    }
                    string query = "USE Projeto_G4 DELETE FROM Estoque WHERE id_produto = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                        cmd.ExecuteNonQuery();
                    }
                    string query2 = "USE Projeto_G4 DELETE FROM cor_produto WHERE id_produto = @id";
                    using (SqlCommand cmd = new SqlCommand(query2, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                        cmd.ExecuteNonQuery();
                    }
                    string query3 = "USE Projeto_G4 DELETE FROM Produtos WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(query3, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                        cmd.ExecuteNonQuery();
                    }
                    if (!Form1.form1.admin)
                    {
                        string queryrelatorio = "USE Projeto_G4 INSERT INTO relatorio (id_funcionario, acao, descricao, datahora) VALUES (@id_funcionario, @acao, @descricao, @datahora)";
                        using (SqlCommand cmd = new SqlCommand(queryrelatorio, conn))
                        {
                            cmd.Parameters.AddWithValue("@id_funcionario", Form1.form1.user);
                            cmd.Parameters.AddWithValue("@acao", "Exclusão");
                            cmd.Parameters.AddWithValue("@descricao", "Produto: " + txtCodigo.Text);
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
            CarregarDados(); //preenche o gridview com os dados do bd
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCodigo.Text.ToString(), out _) && txtCodigo.Text != "") //verifica se codigo é numero inteiro
            {
                MessageBox.Show("O Código deve ser um número inteiro", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 SELECT * FROM Produtos WHERE id LIKE @id AND marca LIKE @marca AND tipo LIKE @tipo AND descricao LIKE @descricao";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (txtCodigo.Text == "") cmd.Parameters.AddWithValue("@id", "%" + txtCodigo.Text + "%");
                        else cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                        cmd.Parameters.AddWithValue("@marca", "%" + txtMarca.Text + "%");
                        cmd.Parameters.AddWithValue("@tipo", "%" + txtTipo.Text + "%");
                        cmd.Parameters.AddWithValue("@descricao", "%" + txtDescricao.Text + "%");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoResizeColumn(0);
                    }
                    string query2 = "USE Projeto_G4 SELECT cor FROM cor_produto WHERE id_produto LIKE @id";
                    using (SqlCommand cmd = new SqlCommand(query2, conn))
                    {
                        if (txtCodigo.Text == "") cmd.Parameters.AddWithValue("@id", "%" + txtCodigo.Text + "%");
                        else cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView2.DataSource = dt;
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimparCampos(); //limpa todas txtbox
            txtCodigo.Text = ProximoId().ToString(); //preenche o campo codigo com o proximo id disponivel
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
                    txtCodigo.Text = dataGridView1.Rows[linha].Cells[0].Value.ToString();
                    txtMarca.Text = dataGridView1.Rows[linha].Cells[1].Value.ToString();
                    txtTipo.Text = dataGridView1.Rows[linha].Cells[2].Value.ToString();
                    txtPreco.Text = dataGridView1.Rows[linha].Cells[3].Value.ToString();
                    txtDescricao.Text = dataGridView1.Rows[linha].Cells[4].Value.ToString();
                    using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                    {
                        string query = "USE Projeto_G4 SELECT cor FROM cor_produto WHERE id_produto = @id";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", txtCodigo.Text);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dataGridView2.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCor_Click(object sender, EventArgs e)
        {
            if (txtCodigo.Text == "")
            {
                MessageBox.Show("Selecione um produto para editar as cores", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtCodigo.Text.ToString(), out _)) //verifica se codigo é numero inteiro
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
            FormCor formCor = new FormCor();
            formCor.ShowDialog();
        }

        private void btnX_Click(object sender, EventArgs e)
        {
            txtCodigo.Text = "";  //limpa o campo codigo
        }

        private void txtCodigo_TextChanged(object sender, EventArgs e)
        {
            //se o campo codigo tiver algum texto, torna visivel o botão com "x"
            if (txtCodigo.Text == "") btnX.Visible = false;
            else btnX.Visible = true;
        }
    }

}