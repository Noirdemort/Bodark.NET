using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using Newtonsoft.Json;

namespace Bodark
{
    public class BlockChain<T>
       where T : class
    {
        readonly List<Block<T>> _blocks = new List<Block<T>>();

        public IReadOnlyCollection<Block<T>> Blocks => _blocks;

        public BlockChain()
        {
            InitializeChain();
        }

        public Block<T> GetLatest()
        {
            return _blocks[_blocks.Count - 1];
        }

        public Block<T> GetBlock(Func<Block<T>, bool> predicate)
        {
            return _blocks.SingleOrDefault(predicate);
        }

        public bool Validate()
        {
            if (_blocks.Count > 1)
            {
                for (int i = 1; i < _blocks.Count; i++)
                {
                    Block<T> currBlock = _blocks[i];
                    Block<T> prevBlock = _blocks[i - 1];

                    if (currBlock.Hash != currBlock.CalculateHash())
                        return false;

                    if (currBlock.Hash != prevBlock.Hash)
                        return false;
                }
            }

            return true;
        }

        public void AddBlock(Block<T> block)
        {
            if (!block.Validate()) throw new InvalidOperationException("invalid block");

            var latestBlock = GetLatest();

            block.Index = latestBlock.Index + 1;
            block.PreviousHash = latestBlock.Hash;

            _blocks.Add(block);
        }

        public void AddBlock(Block<T> block, int difficulty)
        {
            block.Mine(difficulty);

            AddBlock(block);
        }

        void InitializeChain()
        {
            var block = new Block<T>(DateTimeOffset.Now, null);

            _blocks.Add(block);
        }
    }

    public class Block<T>
        where T : class
    {
        readonly List<T> _data = new List<T>();

        public int Index { get; internal set; }

        public DateTimeOffset Timestamp { get; internal set; }

        public string PreviousHash { get; internal set; }

        public string Hash { get; private set; }

        public int Nonce { get; private set; }

        public IReadOnlyCollection<T> Data => _data;

        public Block(DateTimeOffset timestamp, T data)
        {
            Index = 0;
            Timestamp = timestamp;
            PreviousHash = null;
            Nonce = 0;
            _data.Add(data);
            Hash = CalculateHash();
        }

        public bool Validate()
        {
            return Hash == CalculateHash();
        }

        public void AddData(T data)
        {
            _data.Add(data);
            Hash = CalculateHash();
        }

        protected internal string CalculateHash()
        {
            byte[] output;

            using (SHA256 sHA256 = SHA256.Create())
            {
                byte[] input = Encoding.ASCII.GetBytes(
                    $"{Timestamp}-{PreviousHash ?? ""}-{Nonce}-{JsonConvert.SerializeObject(_data)}");
                output = sHA256.ComputeHash(input);
            }

            return Convert.ToBase64String(output);
        }

        protected internal void Mine(int difficulty)
        {
            var leadingZeros = new string('0', difficulty);

            while (Hash == null || Hash.Substring(0, difficulty) != leadingZeros)
            {
                Nonce++;
                Hash = CalculateHash();
            }
        }
    }
}
