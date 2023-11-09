using System;
using System.Collections.Generic;
using System.Text;
using ImageCompression.Interfaces;
using ImageCompression.Common.Haffman;
using System.Collections;
using System.Threading.Tasks;
using ImageCompression.Common;
using System.Threading;

namespace ImageCompression.Algorithms
{
    public class Haffman : IAlgorithm
    {
        TwoWayDictionary<short, BitArray> dictionary;
        const short endOfStream = 256;
        byte maxChainElementKey;
        BitArray maxChainElementValue;

       
        public byte[] Compress(byte[] data)
        {
            SetByDefault();
            LinkedList<Node> query = CreatePriorityQuery(data);

            while (query.Count > 1)
            {
                Node newNode = GetNewNodeFromQuery(query);
                AddNodeToQuery(query, newNode);
            }
            Node rootNode = query.First.Value;
            SetBitTable(rootNode, new BitArray(0));
            AddEndOfStreamToTable();
            BitWriter bitWriter = new BitWriter();
            foreach (byte b in data)
            {
                bitWriter.WriteBitArray(dictionary.GetValueByKey(b));
            }
            bitWriter.WriteBitArray(dictionary.GetValueByKey(endOfStream));
            return bitWriter.GetBytes();
        }
        private void SetByDefault()
        {
            dictionary = new TwoWayDictionary<short, BitArray>(new BitArrayEqualityComparer());
            maxChainElementKey = 0;
            maxChainElementValue = new BitArray(0);
        }
        private LinkedList<Node> CreatePriorityQuery(byte[] data)
        {
            int[] byteFrequencyTable = GetFrequencyTable(data);

            Node[] nodes = new Node[byteFrequencyTable.Length];
            for (int i = 0; i < byteFrequencyTable.Length; i++)
            {
                nodes[i] = new Node() { Value = new object[] { (byte)i }, Weight = byteFrequencyTable[i] };
            }
            //sort nodes by weight and remove nodes with weight = 0
            Array.Sort(nodes, (x, y) => x.Weight.CompareTo(y.Weight));
            //create query by nodes
            return new LinkedList<Node>(nodes);
        }
        private int[] GetFrequencyTable(byte[] data)
        {
            int[] byteFrequencyTable = new int[endOfStream];
            Parallel.ForEach(data, b =>
            {
                Interlocked.Increment(ref byteFrequencyTable[b]);
            });
            return byteFrequencyTable;
        }
        private Node GetNewNodeFromQuery(LinkedList<Node> query)
        {
            //get first element from query and remove it from query
            Node first = query.First.Value;
            query.RemoveFirst();
            //get second element from query and remove it from query
            Node second = query.First.Value;
            query.RemoveFirst();
            //create new node with first and second elements as children
            return new Node()
            {
                Value = new object[] { first, second },
                Weight = first.Weight + second.Weight
            };
        }
        private void AddNodeToQuery(LinkedList<Node> query, Node newNode)
        {
            LinkedListNode<Node> node = query.First;
            while (node != null && node.Value.Weight < newNode.Weight)
            {
                node = node.Next;
            }
            if (node == null)
            {
                query.AddLast(newNode);
            }
            else
            {
                query.AddBefore(node, newNode);
            }
        }
        private void SetBitTable(Node parent, BitArray bitArray)
        {
            if (parent.Value.Length == 1)
            {
                byte code = (byte)parent.Value[0];
                if (maxChainElementValue.Length < bitArray.Length)
                {
                    maxChainElementKey = code;
                    maxChainElementValue = bitArray;
                }
                dictionary.Add(code, bitArray);
            }
            else
            {
                BitArray[] bitArrays = new BitArray[2];
                for (int i = 0; i < 2; i++)
                {
                    bitArrays[i] = new BitArray(bitArray);
                    bitArrays[i].Length++;
                    bitArrays[i][bitArrays[i].Length - 1] = i != 0 ? false : true;
                    SetBitTable((Node)parent.Value[i], bitArrays[i]);
                }
            }
        }
        private void AddEndOfStreamToTable()
        {
            BitArray[] bitArrays = new BitArray[2];
            for (int i = 0; i < 2; i++)
            {
                bitArrays[i] = new BitArray(maxChainElementValue);
                bitArrays[i].Length++;
                bitArrays[i][bitArrays[i].Length - 1] = i != 0 ? false : true;
            }
            dictionary.Update(maxChainElementKey, bitArrays[0]);
            dictionary.Add(endOfStream, bitArrays[1]);
        }

        public byte[] Decompress(byte[] data)
        {
            if (dictionary.Count == 0)
            {
                throw new Exception("Dictionary is empty");
            }
            BitReader bitReader = new BitReader(data);
            BitArray bitArray = new BitArray(0);
            List<byte> result = new List<byte>();
            while (!bitReader.IsEnd())
            {
                bitArray.Length++;
                bitArray[bitArray.Length - 1] = bitReader.ReadBit();
                if (dictionary.ContainsValue(bitArray))
                {
                    short b = dictionary.GetKeyByValue(bitArray);
                    if (b == endOfStream)
                    {
                        break;
                    }
                    result.Add((byte)b);
                    bitArray = new BitArray(0);
                }
            }
            return result.ToArray();
        }
    }

}
