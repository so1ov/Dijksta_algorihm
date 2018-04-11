using System;
using System.Collections;
using System.IO;

namespace Ulyanov_101217_findPathWithoutMinusLoops
{
    class Graph
    {
        static Random randomGenerator = new Random();
        /** Список vertexes_ вершин экземпляра графа хранит информацию о каждой вершине
         * и исходящих из неё дугах (ориентированных ребрах).
         * Это единственное поле класса.
         */
        private System.Collections.Generic.List<Node> vertexes_;

        /** Вложенный класс Node реализует единичный элемент списка vertexes_,
         * описывающий одну вершину графа.
         * Публичное поле name хранит название вершины, закрытый список вершин neighbours_
         * содержит названия вершин, к которым исходят дуги из данной вершины.
         */
        class Node : IEnumerator, IEnumerable
        {
            public string name;
            private System.Collections.Generic.List<string> neighbours_;
            int position = -1;

            public bool isEven
            {
                get
                {
                    return neighbours_.Count % 2 == 0;
                }
            }

            public int Count
            {
                get
                {
                    return neighbours_.Count;
                }
            }

            public string this[int i]
            {
                get
                {
                    return neighbours_[i];
                }
                set
                {
                    neighbours_[i] = value;
                }
            }



            /** Публичный конструктор Node(string) принимает строку вида:
             * a:b,c
             * где a это название новой вершины, b и c - названия всех вершин, к которым
             * от неё исходят дуги (вершин-"соседей").
             */
            public Node(string parse)
            {
                char[] delims =
                {
                    ':',
                    ','
                };
                string[] parseResult = parse.Split(delims);
                name = parseResult[0];
                neighbours_ = new System.Collections.Generic.List<string>();
                for (int i = 1; i < parseResult.Length; i++)
                {
                    neighbours_.Add(parseResult[i]);
                }

            }

            /** Публичный метод print() выводит название вершины и всех исходящих из неё дуг
             * на экран.
             */
            public void print()
            {
                if (neighbours_.Count == 0)
                {
                    Console.WriteLine("- Вершина {0} не имеет исходящих дуг.", name);
                    return;
                }

                Console.WriteLine("- Вершина {0}:", name);
                Console.WriteLine("Исходящие дуги:");
                foreach (var i in neighbours_)
                {
                    Console.WriteLine("{0} -> {1}", name, i);
                }
            }

            /** Метод addEdge(string) добавляет в список вершин-"соседей" вершину
             * с заданным именем, т.е. создает новую дугу, исходящую из this
             * в new Edge.
             */
            public void addEdge(string newEdge)
            {
                neighbours_.Add(newEdge);
            }

            /** Метод deleteEdge(string) удаляет из списка вершин-"соседей" this
             * все вхождения deletedEdge, т.е. удаляет все дуги, исходящие из this
             * в deletedEdge.
             */
            public void deleteEdge(string deletedEdge)
            {
                while (neighbours_.Remove(deletedEdge)) ;
            }

            /** Метод findLoop() выполняет роль функции-предиката,
             * возвращающей true в случае если вершина содержит себя в списке
             * вершин-"соседей", то есть в случае если вершина имеет петлю,
             * и false в случае если вершина не имеет петель.
             * Метод используется при поиске петель в графе, его вызов производится
             * для каждой вершины в графе (для каждого элемента списка vertexes_).
             */
            public bool findLoop()
            {
                foreach (var i in neighbours_)
                {
                    if (name == i)
                    {
                        return true;
                    }
                }
                return false;
            }

            public bool findNeighbour(string search)
            {
                foreach(var i in neighbours_)
                {
                    if(i == search)
                    {
                        return true;
                    }
                }
                return false;
            }

            /** Метод deleteRandomEdge удаляет из числа вершин-соседей данной вершины случайный элемент,
             * таким образом удаляя случайную дугу, начинающуюся с вершины this.
             */
            public void deleteRandomEdge()
            {
                if (neighbours_.Count > 0)
                {
                    neighbours_.Remove(neighbours_[randomGenerator.Next(0, neighbours_.Count)]);
                }
            }

            /** Здесь и далее - методы, являющиеся частью реализации интерфейсов IEnumerator и IEnumerable,
             * то есть позволяющие получать доступ к элементам коллекции vertexes_ по индексу.
             */
            public IEnumerator GetEnumerator()
            {
                return this;
            }

            public bool MoveNext()
            {
                position++;
                return (position < neighbours_.Count);
            }

            public void Reset()
            {
                position = 0;
            }

            public object Current
            {
                get { return neighbours_[position]; }
            }

        } /** Конец реализации класса вершины Node */

        /** Публичный конструктор класса Graph() создает "пустой" граф -
         * граф, не имеющий ни вершин, ни дуг, инициализируя поле vertexes_ пустым списком вершин.
         */
        public Graph()
        {
            vertexes_ = new System.Collections.Generic.List<Node>();
        }

        /** Публичный конструктор класса Graph(string) создает граф,
         * заданный файлом filename.
         * Файл имеет вид:
         * a:b,c
         * b:c,a
         * c:a,b
         * где a, b и с - вершины графа, а запись вида
         * a:b,c
         * означает что вершина a имеет две исходящие дуги: одна из a к b, другая из a к c.
         */
        public Graph(string filename)
        {
            vertexes_ = new System.Collections.Generic.List<Node>();
            StreamReader fileReader = new StreamReader(filename);
            string stringBuff;
            while ((stringBuff = fileReader.ReadLine()) != null)
            {
                addNode(stringBuff);
            }
        }

        /** Публичный конструктор Graph(Graph) реализует глубокое копирование
         * уже имеющегося объекта графа для независимого (не изменяющего состояние оригинала)
         * выполнения операций над копией.
         */
        public Graph(Graph source)
        {
            vertexes_ = new System.Collections.Generic.List<Node>();
            foreach (var i in source.vertexes_)
            {
                vertexes_.Add(i);
            }
        }

        /** Публичный конструктор Graph(int) создает полный граф с числами-названиями вершин по умолчанию. */
        public Graph(int num)
        {
            vertexes_ = new System.Collections.Generic.List<Node>();
            string buff;
            for (int i = 0; i < num; i++)
            {
                buff = Convert.ToString(i) + ":";
                for (int j = 0; j < num; j++)
                {
                    if (j != i)
                    {
                        buff += Convert.ToString(j) + ',';
                    }
                }
                if (buff[buff.Length - 1] == ',')
                {
                    buff = buff.Remove(buff.Length - 1);
                }
                addNode(buff);
            }
        }

        /** Метод класса Graph addNode(string) добавляет в список вершин новую, принимая строку из файла,
         * описывающего граф, или строку того же формата
         * ([вершина-источник дуги]:[вершина-приемник],[вершина-приемник]).
         */
        public void addNode(string parse)
        {
            vertexes_.Add(new Node(parse));
        }

        /** Метод класса Graph deleteNode(string) удаляет из списка вершин
         * вершину с заданным именем nodeName, а также все её упоминания в списках вершин-"соседей"
         * остальных вершин.
         */
        public void deleteNode(string nodeName)
        {
            for (var i = 0; i < vertexes_.Count; i++)
            {
                if (nodeName == vertexes_[i].name)
                {
                    /** При удалении вершины с заданным именем длина списка уменьшается на 1,
                     * и счетчик i указывает теперь на следующий, необработанный по условию if
                     * элемент списка. Счетчик декрементируется после завершения работы
                     * метода Remove, чтобы не пропустить удаление упоминаний искомой вершины во
                     * впередистоящем элементе списка.
                     */
                    vertexes_.Remove(vertexes_[i--]);
                }
                else
                {
                    vertexes_[i].deleteEdge(nodeName);
                }

            }
        }

        /** Метод класса Graph addEdge(string, string) добавляет в число вершин-"соседей"
         * вершины sourceNode вершину destinationNode, т.е. создаёт дугу из вершины sourceNode
         * в вершину destinationNode.
         */
        public void addEdge(string sourceNode, string destinationNode)
        {
            /** Для поиска вершины-"источника" (вершины, из которой исходит создаваемая дуга)
             * применяется предикат "element => element.name == sourceNode",
             * применяемый методом Find ко всем элементам списка vertexes_ и
             * метод Node.addEdge вызывается только для той вершины, которая имеет
             * название sourceNode.
             */
            vertexes_.Find(element => element.name == sourceNode).addEdge(destinationNode);
        }

        /** Метод класса Graph deleteEdge(string, string) удаляет все имеющиеся в графе
         * дуги с началом в вершине sourceNode и концом в вершине destinationNode.
         */
        public void deleteEdge(string sourceNode, string destinationNode)
        {
            vertexes_.Find(element => element.name == sourceNode).deleteEdge(destinationNode);
        }

        /** Метод класса Graph print() выводит информацию обо всех вершинах
         * (название и список вершин-"соседей") на экран.
         */
        public void print()
        {
            Console.WriteLine("Ориентированный граф:");
            foreach (var i in vertexes_)
            {
                i.print();
            }
            Console.WriteLine();
        }

        /** Метод класса Graph deleteRandomEdge() выбирает случайную
         * начальную вершину из диапазона вершин графа, и вызывает для нее метод класса вершины Node
         * deleteRandomEdge, удаляющий из массива соседей вершины случайный элемент.
         */
        public void deleteRandomEdge()
        {
            if (vertexes_.Count > 0)
            {
                vertexes_[randomGenerator.Next(0, vertexes_.Count)].deleteRandomEdge();
            }
        }

        /** Метод класса Graph deleteRandomEdges() является оберткой метода
         * deleteRandomEdge, вызывающей данный метод k раз.
         */
        public void deleteRandomEdges(int k)
        {
            for (int i = 0; i < k; i++)
            {
                deleteRandomEdge();
            }
        }

        /** Метод класса Graph findPathExcludeVertexes() удаляет из графа k случайных ребер,
         * затем выполняет поиск в ширину в получившемся орграфе, и выводит на экран данные
         * о существовании пути из вершины start в вершину finish.
         */
        public void findPathExcludeVertexes(string start, string finish, int k)
        {
            if (k > vertexes_.Count)
            {
                Console.WriteLine("Число элементов, представленных к удалению, больше числа элементов графа.");
                return;
            }
            if (vertexes_.Find(element => element.name == start) == null)
            {
                Console.WriteLine("Не найдена вершина-источник.");
                return;
            }
            if (vertexes_.Find(element => element.name == finish) == null)
            {
                Console.WriteLine("Не найдена вершина-приемник.");
                return;
            }

            deleteRandomEdges(k);

            var visitedNodes = new System.Collections.Generic.List<Node>();
            visitedNodes.Add(vertexes_.Find(element => element.name == start));
            for (var i = 0; i < visitedNodes.Count; i++)
            {
                if (visitedNodes[i].name == finish)
                {
                    Console.WriteLine("Вершина " + finish + " достижима из вершины " + start + ".");
                    return;
                }

                foreach (var j in visitedNodes[i])
                {
                    if (!visitedNodes.Contains(vertexes_.Find(element => element.name == (string)j)))
                    {
                        visitedNodes.Add(vertexes_.Find(element => element.name == (string)j));
                    }
                }
            }

            Console.WriteLine("Вершина " + finish + " недостижима из вершины " + start + ".");
            return;
        }

        /** Метод deleteEvenVertexes выполняет удаление вершин с четными степенями, используя метод deleteNode.
         */
        public void deleteEvenVertexes()
        {
            System.Collections.Generic.List<bool> marks = new System.Collections.Generic.List<bool>();
            for (var i = 0; i < vertexes_.Count; i++)
            {
                marks.Add(false);
                if (vertexes_[i].isEven)
                {
                    marks[i] = true;
                }
            }

            for (var i = marks.Count - 1; i >= 0; i--)
            {
                if (marks[i] == true)
                {
                    deleteNode(vertexes_[i].name);
                }
            }
        }

        /** Метод findPathByDijkstra выполняет поиск пути в графе с ограничением максимального уровня 
         * поиска (глубины вложения вершины).
         */
        public void findPathByDijkstra(string start, string finish, int limit)
        {
            Node currNode = vertexes_.Find(x => x.name == start);
            bool[] passed = new bool[vertexes_.Count];
            int[] pathLengths = new int[vertexes_.Count];
            int[] parents = new int[vertexes_.Count];
            for(int i = 0; i < vertexes_.Count; i++)
            {
                if(vertexes_[i] == currNode)
                {
                    pathLengths[i] = 0;
                }
                else
                {
                    pathLengths[i] = int.MaxValue;
                }
            }

            int buff = 0;
            bool isIterLeft = false;
            int carriage = 0;
            do
            {
                isIterLeft = false;
                for(int i = 0; i < currNode.Count; i++)
                {
                    if(passed[vertexes_.FindIndex(x => x.name == currNode[i])])
                    {
                        continue;
                    }

                    buff = pathLengths[vertexes_.FindIndex(x => x.name == currNode.name)];
                    if (pathLengths[vertexes_.FindIndex(x => x.name == currNode[i])] > buff + 1)
                    {
                        parents[vertexes_.FindIndex(x => x.name == currNode[i])] = vertexes_.FindIndex(x => x.name == currNode.name);
                        pathLengths[vertexes_.FindIndex(x => x.name == currNode[i])] = buff + 1;
                    }
                }
                passed[vertexes_.FindIndex(x => x.name == currNode.name)] = true;
                for(carriage = 0; carriage < vertexes_.Count; carriage++)
                {
                    if(passed[carriage] == false)
                    {
                        currNode = vertexes_[carriage];
                        isIterLeft = true;
                        break;
                    }
                }
            }
            while (isIterLeft);

            if(pathLengths[vertexes_.FindIndex(x => x.name == finish)] <= limit)
            {
                Console.WriteLine("Найден путь между " + start + " и " + finish + ", не выходящий за пределы лимита длины " + limit + ":");
                string current = finish;
                Console.Write(current);
                while (current != start)
                {
                    Console.Write(" <- ");
                    current = vertexes_[parents[vertexes_.FindIndex(x => x.name == current)]].name;
                    Console.Write(current);
                }
            }
            else
            {
                Console.WriteLine("Путей, отвечающих заданным требованиям, не существует.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var gr = new Graph("input1.txt");
            gr.findPathByDijkstra("a", "d", 2);
            Console.ReadKey();
        }
    }
}
