using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoSenac
{
    public partial class FormCliente : Form
    {
        public FormCliente()
        {
            InitializeComponent();
            txtId.Text = ProximoId().ToString(); //preenche o campo codigo com o proximo id disponivel
            CarregarDados(); //preenche o gridview com os dados do bd
        }

        //chamando a variavel com o endereço da conexão com o bd
        private static string connectionString = Program.connectionString;

        private void CarregarDados() //metodo q preenche o gridview com os dados do bd
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    string query = "USE Projeto_G4 SELECT * FROM clientes";
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
                        string query = "USE Projeto_G4 SELECT * FROM clientes WHERE id = @id";
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

        private void LimparCampos() //limpa todas txtbox
        {
            txtId.Clear();
            txtCPF.Clear();
            txtTelefone.Clear();
            txtNome.Clear();
            txtEmail.Clear();
        }

        private bool ChecarCpf() //checa se todos os 14 caracteres foram preenchidos
        {
            if (txtCPF.Text.Length < 14)
            {
                return false;
            }
            else return true;
        }

        private string CpfUnico() //garante q o cpf inserido seja unico
        {
            try
            {
                string id = "";
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 SELECT * FROM clientes WHERE cpf = @cpf";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@cpf", txtCPF.Text);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoResizeColumn(0);
                        if (dataGridView1.RowCount.ToString() != "0") //se cpf já existe no bd, retorna o id desse cliente
                        {
                            id = dataGridView1.Rows[0].Cells[0].Value.ToString();
                            return id;
                        }
                        return "0"; //se cpf nao existe no bd, retorna 0
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
                    string query = "USE Projeto_G4 SELECT * FROM clientes WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtId.Text);
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
            if (txtNome.Text == "" || txtCPF.Text == "")
            {
                CarregarDados();
                MessageBox.Show("Um ou mais campos obrigatórios estão em branco", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            else return false;
        }

        private void txtId_TextChanged(object sender, EventArgs e)
        {
            //se o campo codigo tiver algum texto, torna visivel o botão com "x"
            if (txtId.Text == "") btnX.Visible = false;
            else btnX.Visible = true;
        }

        private void btnX_Click(object sender, EventArgs e)
        {
            txtId.Text = ""; //limpa o campo codigo
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
                using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                {
                    conn.Open();
                    string query = "USE Projeto_G4 SELECT * FROM clientes WHERE id LIKE @id AND cliente LIKE @cliente AND cpf LIKE @cpf AND telefone LIKE @telefone AND email LIKE @email";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (txtId.Text == "") cmd.Parameters.AddWithValue("@id", "%" + txtId.Text + "%");
                        else cmd.Parameters.AddWithValue("@id", txtId.Text);
                        cmd.Parameters.AddWithValue("@cliente", "%" + txtNome.Text + "%");
                        cmd.Parameters.AddWithValue("@email", "%" + txtEmail.Text + "%");
                        //linhas abaixo tiram os caracteres especiais para permitir pesquisa em caso de campo vazio 
                        if (txtTelefone.Text == "(  )      -") cmd.Parameters.AddWithValue("@telefone", "%");
                        else cmd.Parameters.AddWithValue("@telefone", "%" + txtTelefone.Text + "%");
                        if (txtCPF.Text == "   ,   ,   -") cmd.Parameters.AddWithValue("@cpf", "%");
                        else cmd.Parameters.AddWithValue("@cpf", "%" + txtCPF.Text + "%");
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text.ToString(), out _)) //verifica se codigo é numero inteiro
            {
                MessageBox.Show("O Código deve ser um número inteiro", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Checar_Vazio()) return; //se um campo estiver vazio, não faz o insert
            //so faz o insert se cpf e id ainda nao existirem no bd e se o cpf for valido (14 caracteres)
            if (CpfUnico() == "0" && IdUnico() && ChecarCpf())
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                    {
                        conn.Open();
                        string query = "USE Projeto_G4 INSERT INTO clientes (id, cliente, cpf, telefone, email) VALUES (@id, @cliente, @cpf, @telefone, @email)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", txtId.Text);
                            cmd.Parameters.AddWithValue("@cliente", txtNome.Text);
                            cmd.Parameters.AddWithValue("@cpf", txtCPF.Text);
                            //linha abaixo tira os caracteres especiais se o campo telefone estiver vazio
                            if (txtTelefone.Text == "(  )      -") cmd.Parameters.AddWithValue("@telefone", "");
                            else cmd.Parameters.AddWithValue("@telefone", txtTelefone.Text);
                            cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                            cmd.ExecuteNonQuery();
                        }
                        if (!Form1.form1.admin)
                        {
                            string queryrelatorio = "USE Projeto_G4 INSERT INTO relatorio (id_funcionario, acao, descricao, datahora) VALUES (@id_funcionario, @acao, @descricao, @datahora)";
                            using (SqlCommand cmd = new SqlCommand(queryrelatorio, conn))
                            {
                                cmd.Parameters.AddWithValue("@id_funcionario", Form1.form1.user);
                                cmd.Parameters.AddWithValue("@acao", "Cadastro");
                                cmd.Parameters.AddWithValue("@descricao", "Cliente: " + txtId.Text);
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
            else if (!ChecarCpf()) MessageBox.Show("CPF inválido", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else MessageBox.Show("Esse CPF já está cadastrado", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            CarregarDados(); //preenche o gridview com os dados do bd
        }

        private void bntAlterar_Click(object sender, EventArgs e)
        {
            if (txtId.Text == "")
            {
                MessageBox.Show("Nenhum cliente selecionado", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtId.Text.ToString(), out _)) //verifica se codigo é numero inteiro
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
            string id = CpfUnico();
            if ((id == "0" || id == txtId.Text) && ChecarCpf())
            //so permite o update se o cpf for novo ou igual ao atual
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString)) //cria conexão com o bd
                    {
                        conn.Open();
                        string query = "USE Projeto_G4 UPDATE clientes SET cliente = @cliente, cpf = @cpf, telefone = @telefone, email = @email WHERE id = @id ";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@cliente", txtNome.Text);
                            cmd.Parameters.AddWithValue("@cpf", txtCPF.Text);
                            //linha abaixo tira os caracteres especiais se o campo telefone estiver vazio
                            if (txtTelefone.Text == "(  )      -") cmd.Parameters.AddWithValue("@telefone", "");
                            else cmd.Parameters.AddWithValue("@telefone", txtTelefone.Text);
                            cmd.Parameters.AddWithValue("@email", txtEmail.Text);
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
                                cmd.Parameters.AddWithValue("@descricao", "Cliente: " + txtId.Text);
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
            else if (!ChecarCpf()) MessageBox.Show("CPF inválido", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else MessageBox.Show("Esse CPF já está cadastrado", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            CarregarDados(); //preenche o gridview com os dados do bd
        }

        private void btnDeletar_Click(object sender, EventArgs e)
        {
            if (txtId.Text == "")
            {
                MessageBox.Show("Nenhum cliente selecionado", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    string query = "USE Projeto_G4 DELETE FROM venda_cliente WHERE id_cliente = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", txtId.Text);
                        cmd.ExecuteNonQuery();
                    }
                    string query2 = "USE Projeto_G4 DELETE FROM clientes WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(query2, conn))
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
                            cmd.Parameters.AddWithValue("@descricao", "Cliente: " + txtId.Text);
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

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimparCampos(); //limpa todas txtbox
            txtId.Text = ProximoId().ToString(); //preenche o campo codigo com o proximo id disponivel
            CarregarDados(); //preenche o gridview com os dados do bd
        }

        private void txtTelefone_Click(object sender, EventArgs e)
        {
            //se campo vazio, move cursor pro comeco da linha
            if (txtTelefone.Text == "(  )      -") txtTelefone.Select(0, 0);
        }

        private void txtCPF_Click(object sender, EventArgs e)
        {
            //se campo vazio, move cursor pro comeco da linha
            if (txtCPF.Text == "   ,   ,   -") txtCPF.Select(0, 0);
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
                    txtNome.Text = dataGridView1.Rows[linha].Cells[1].Value.ToString();
                    txtCPF.Text = dataGridView1.Rows[linha].Cells[2].Value.ToString();
                    txtTelefone.Text = dataGridView1.Rows[linha].Cells[3].Value.ToString();
                    txtEmail.Text = dataGridView1.Rows[linha].Cells[4].Value.ToString();
                }
            }
            catch (Exception ex) //caso der erro, mostra a mensagem de erro
            {
                MessageBox.Show("Erro: " + ex, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
