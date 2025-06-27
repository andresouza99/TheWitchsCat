using System;
using System.Collections;
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
using static System.Windows.Forms.LinkLabel;

namespace ProjetoSenac
{
    public partial class FormFuncionario : Form
    {
        public FormFuncionario()
        {
            InitializeComponent();
            txtId.Text = ProximoId().ToString(); //preenche o campo codigo com o proximo id disponivel
            CarregarDados(); //preenche o gridview com os dados do bd
        }

        //chamando a variavel com o endereço da conexão com o bd
        private readonly string connectionString = Program.connectionString;

        private void CarregarDados() //metodo q preenche o gridview com os dados do bd
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    string query = "USE Projeto_G4 SELECT id, tipo, usuario FROM funcionarios";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoResizeColumn(0);
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
                        string query = "USE Projeto_G4 SELECT id, tipo, usuario FROM funcionarios WHERE id = @id";
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
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return 1;
        }

        private string ChecaTipo() //checa qual radiobox está selecionada
        {
            if (radioGerente.Checked == true) return "Gerente";
            else return "Vendedor";
        }

        private void LimparCampos() //limpa todas txtbox e volta o radiobutton pra "vendedor"
        {
            txtId.Clear();
            txtUsuario.Clear();
            txtSenha.Clear();
            radioVendedor.Checked = true;
            radioGerente.Checked = false;
        }

        private string UsuarioUnico() //garante q o usuario inserido seja unico
        {
            try
            {
                string id = "";
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 SELECT id, tipo, usuario FROM funcionarios WHERE usuario = @usuario";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuario", txtUsuario.Text);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoResizeColumn(0);
                        if (dataGridView1.RowCount.ToString() != "0") //se usuario já existe no bd, retorna o id desse usuario
                        {
                            id = dataGridView1.Rows[0].Cells[0].Value.ToString();
                            return id;
                        }
                        return "0"; //se usuario nao existe no bd, retorna 0
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        private Boolean IdUnico() //impossibilita insert com id q ja esta em uso
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 SELECT id, tipo, usuario FROM funcionarios WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtId.Text);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoResizeColumn(0);
                        if (dataGridView1.RowCount.ToString() != "0") //se id ja esta em uso, retorna false
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
            if (txtUsuario.Text == "" || txtSenha.Text == "")
            {
                CarregarDados();
                MessageBox.Show("Um ou mais campos estão em branco", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            else return false;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimparCampos(); //limpa todas txtbox
            txtId.Text = ProximoId().ToString(); //preenche o campo codigo com o proximo id disponivel
            CarregarDados(); //preenche o gridview com os dados do bd
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text.ToString(), out _)) //verifica se codigo é numero inteiro
            {
                MessageBox.Show("O Código deve ser um número inteiro", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Checar_Vazio()) return; //se um campo estiver vazio, não faz o insert
            if (UsuarioUnico() == "0" && IdUnico()) //so faz o insert se usuario e id ainda nao existirem no bd
            {
                try
                {
                    string tipo = ChecaTipo(); //checa qual radiobox está selecionada
                    using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                    {
                        conn.Open();
                        string query = "USE Projeto_G4 INSERT INTO funcionarios (id, tipo, usuario, senha) VALUES (@id, @tipo, @usuario, @senha)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", txtId.Text);
                            cmd.Parameters.AddWithValue("@tipo", tipo);
                            cmd.Parameters.AddWithValue("@usuario", txtUsuario.Text);
                            cmd.Parameters.AddWithValue("@senha", txtSenha.Text);
                            cmd.ExecuteNonQuery();
                        }
                        if (!Form1.form1.admin)
                        {
                            string queryrelatorio = "USE Projeto_G4 INSERT INTO relatorio (id_funcionario, acao, descricao, datahora) VALUES (@id_funcionario, @acao, @descricao, @datahora)";
                            using (SqlCommand cmd = new SqlCommand(queryrelatorio, conn))
                            {
                                cmd.Parameters.AddWithValue("@id_funcionario", Form1.form1.user);
                                cmd.Parameters.AddWithValue("@acao", "Cadastro");
                                cmd.Parameters.AddWithValue("@descricao", "Funcionário: " + txtId.Text);
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
            else MessageBox.Show("Esse usuário já existe", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            CarregarDados(); //preenche o gridview com os dados do bd
        }

        private void bntAlterar_Click(object sender, EventArgs e)
        {
            if (txtId.Text == "")
            {
                MessageBox.Show("Nenhum funcionário selecionado", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtId.Text.ToString(), out _)) //verifica se codigo é numero inteiro
            {
                MessageBox.Show("O Código deve ser um número inteiro", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtUsuario.Text == "") //se o campo estiver vazio, não faz o update
            {
                MessageBox.Show("Um ou mais campos estão em branco", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (IdUnico()) //precisa de um id existente para fazer o update
            {
                MessageBox.Show("Código inválido", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CarregarDados(); //preenche o gridview com os dados do bd
                return;
            }
            string tipo = ChecaTipo(); //checa qual radiobox está selecionada
            string id = UsuarioUnico();
            if (id == "0" || id == txtId.Text) //so permite o update se o usuario for novo ou igual ao atual
            {
                if (txtSenha.Text == "")
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                        {
                            conn.Open();
                            string query = "USE Projeto_G4 UPDATE funcionarios SET tipo = @tipo, usuario = @usuario WHERE id = @id ";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@tipo", tipo);
                                cmd.Parameters.AddWithValue("@usuario", txtUsuario.Text);
                                cmd.Parameters.AddWithValue("@id", txtId.Text);
                                cmd.ExecuteNonQuery();
                            }
                            if (!Form1.form1.admin)
                            {
                                string queryrelatorio = "USE Projeto_G4 INSERT INTO relatorio (id_funcionario, acao, descricao, datahora) VALUES (@id_funcionario, @acao, @descricao, @datahora)";
                                using (SqlCommand cmd = new SqlCommand(queryrelatorio, conn))
                                {
                                    cmd.Parameters.AddWithValue("@id_funcionario", Form1.form1.user);
                                    cmd.Parameters.AddWithValue("@acao", "Atualização");
                                    cmd.Parameters.AddWithValue("@descricao", "Funcionário: " + txtId.Text);
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
                }
                else
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                        {
                            conn.Open();
                            string query = "USE Projeto_G4 UPDATE funcionarios SET tipo = @tipo, usuario = @usuario, senha = @senha WHERE id = @id ";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@tipo", tipo);
                                cmd.Parameters.AddWithValue("@usuario", txtUsuario.Text);
                                cmd.Parameters.AddWithValue("@senha", txtSenha.Text);
                                cmd.Parameters.AddWithValue("@id", txtId.Text);
                                cmd.ExecuteNonQuery();
                            }
                            if (!Form1.form1.admin)
                            {
                                string queryrelatorio = "USE Projeto_G4 INSERT INTO relatorio (id_funcionario, acao, descricao, datahora) VALUES (@id_funcionario, @acao, @descricao, @datahora)";
                                using (SqlCommand cmd = new SqlCommand(queryrelatorio, conn))
                                {
                                    cmd.Parameters.AddWithValue("@id_funcionario", Form1.form1.user);
                                    cmd.Parameters.AddWithValue("@acao", "Atualização");
                                    cmd.Parameters.AddWithValue("@descricao", "Funcionário: " + txtId.Text);
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
                }
            }
            else MessageBox.Show("Esse usuário já existe", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    txtUsuario.Text = dataGridView1.Rows[linha].Cells[2].Value.ToString();
                    string tipo = dataGridView1.Rows[linha].Cells[1].Value.ToString();
                    //marca o radiobutton correspondente
                    if (tipo == "Vendedor")
                    {
                        radioVendedor.Checked = true;
                        radioGerente.Checked = false;
                    }
                    else
                    {
                        radioVendedor.Checked = false;
                        radioGerente.Checked = true;
                    }
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text.ToString(), out _) && txtId.Text != "") //verifica se codigo é numero inteiro
            {
                MessageBox.Show("O Código deve ser um número inteiro", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                string tipo = ChecaTipo(); //vê o tipo de funcionario q esta selecionado e guarda na variavel "tipo"
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 SELECT id, tipo, usuario FROM funcionarios WHERE id LIKE @id AND tipo LIKE @tipo AND usuario LIKE @usuario";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (txtId.Text == "") cmd.Parameters.AddWithValue("@id", "%" + txtId.Text + "%");
                        else cmd.Parameters.AddWithValue("@id", txtId.Text);
                        cmd.Parameters.AddWithValue("@tipo", "%" + tipo + "%");
                        cmd.Parameters.AddWithValue("@usuario", "%" + txtUsuario.Text + "%");
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

        private void btnDeletar_Click(object sender, EventArgs e)
        {
            if (txtId.Text == "")
            {
                MessageBox.Show("Nenhum funcionário selecionado", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    string query = "USE Projeto_G4 DELETE FROM funcionarios WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtId.Text);
                        cmd.ExecuteNonQuery();
                    }
                    if (!Form1.form1.admin)
                    {
                        string queryrelatorio = "USE Projeto_G4 INSERT INTO relatorio (id_funcionario, acao, descricao, datahora) VALUES (@id_funcionario, @acao, @descricao, @datahora)";
                        using (SqlCommand cmd = new SqlCommand(queryrelatorio, conn))
                        {
                            cmd.Parameters.AddWithValue("@id_funcionario", Form1.form1.user);
                            cmd.Parameters.AddWithValue("@acao", "Exclusão");
                            cmd.Parameters.AddWithValue("@descricao", "Funcionário: " + txtId.Text);
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

        private void btnX_Click(object sender, EventArgs e)
        {
            txtId.Text = ""; //limpa o campo codigo
        }

        private void txtId_TextChanged(object sender, EventArgs e)
        {
            //se o campo codigo tiver algum texto, torna visivel o botão com "x"
            if (txtId.Text == "") btnX.Visible = false;
            else btnX.Visible = true;
        }
    }
}
