using System;
using Microsoft.AspNetCore.Http.Features;

namespace projeto.Models {

    public class Pedido {
        private string id;
        private Itens item;
        private int qtdItens;


        public Pedido(string id, Itens item, int qtdItens) {
            this.id = id;
            this.item = item;
            this.qtdItens = qtdItens;
        }   

        public Pedido() {

        }

        public Itens Item{
            get { return item; }
            set { item = value; }
        }

        public string Id{
            get { return id; }
            set { id = value; }
        }

        public int QtdItens{
            get { return qtdItens; }
            set { qtdItens = value; }
        }

    }
}