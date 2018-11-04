using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheWritersNet
{
    public class Map
    {
        public List<Node> graph = new List<Node>();
        public bool multigraph;
        public bool directed;
        public List<Link> links;
        public List<Node> nodes;
    }
}