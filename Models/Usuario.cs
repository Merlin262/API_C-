using System;

namespace projeto.Models
{
    public class Usuario   
    {
        private string nome;
        private string cpf;
        private List<Pedido> listaPedidos;
        

        public Usuario()
        {
            
        }
        public Usuario(string nome, string cpf, List<Pedido> listaPedidos)
        {
            this.nome = nome;
            this.cpf = cpf;
            this.listaPedidos = listaPedidos;
            
        }

        public string Nome{
            get { return nome; }
            set { nome = value; }
        }

        public string CPF{
            get { return cpf; }
            set { cpf = value; }
        }
        public List<Pedido> ListaPedidos{
            get { return listaPedidos; }
            set { listaPedidos = value; }
        }

        public List<Pedido> DeletarPedido(string id) {
            Pedido pedidoParaRemover = listaPedidos.FirstOrDefault(p => p.Id == id);
            if (pedidoParaRemover != null) {
                listaPedidos.Remove(pedidoParaRemover);
                System.Console.WriteLine("Pedido removido com sucesso");
            } else {
                System.Console.WriteLine("Pedido não encontrado");
            }
            return listaPedidos;
        }
    }
}