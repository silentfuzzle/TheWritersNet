using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWritersNetLogic.HyperlinkParsers
{
    abstract class HyperlinkParser
    {
        public int Index { get; private set; }
        public int[] Brackets { get; private set; }
        public string Markdown { get; set; }

        public void Execute(string markdown)
        {
            Index = 0;
            Brackets = new int[2];
            this.Markdown = markdown;

            int lastIndex = 0;
            string nextSymbol = "[";
            char hyperlinkType = 'x';

            while (Index < Markdown.Length)
            {
                Index = Markdown.IndexOf(nextSymbol, Index);
                if (Index == -1)
                {
                    if (nextSymbol == "[")
                        Index = Markdown.Length;
                    else
                    {
                        nextSymbol = "[";
                        Index = lastIndex;
                    }
                }
                else
                {
                    if (nextSymbol == "[")
                    {
                        nextSymbol = "]";
                        Brackets[0] = Index;
                        lastIndex = Index + 1;

                        if (Index > 0)
                            hyperlinkType = Markdown[Index - 1];
                    }
                    else if (nextSymbol == "]")
                    {
                        if (Index < Markdown.Length - 1 && Markdown[Index + 1] == '(')
                        {
                            nextSymbol = ")";
                            Brackets[1] = Index;
                        }
                        else
                        {
                            nextSymbol = "[";
                            Index = lastIndex;
                        }
                    }
                    else
                    {
                        string hyperlink = Markdown.Substring(Brackets[1] + 2, Index - (Brackets[1] + 2));
                        ProcessHyperlink(hyperlink, hyperlinkType);

                        nextSymbol = "[";
                    }
                }
            }
        }

        protected abstract void ProcessHyperlink(string hyperlink, char hyperlinkType);
    }
}
