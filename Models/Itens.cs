using System;

namespace projeto.Models {

    public class Itens {
        private string nomeItem; 
        private int qtd; 
        
        public Itens() {

        }

        public Itens(string nome, int qtd) {
            this.nomeItem = nome;
            this.qtd = qtd;
        }

        public int Qtd{
            get { return qtd; }
            set { qtd = value; }
        }

        public string NomeItem{
            get { return nomeItem; }
            set { nomeItem = value; }
        }

        public void DecrementarEstoque()
        {
            qtd--;
        }

        public void AcrementarEstoque()
        {
            qtd++;
        }

    }
}