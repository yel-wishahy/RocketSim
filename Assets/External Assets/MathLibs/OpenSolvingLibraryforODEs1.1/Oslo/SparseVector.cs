using System;

namespace Microsoft.Research.Oslo
{
    public struct SparseVector
    {
        const int IncrementSize = 16; // Size of chunk for array increments

        private int n;

        /// <summary>The nonzero elements of the sparse vector</summary>
        public double[] items;
        /// <summary>The indices of the nonzero elements in the sparse vector</summary>
        public int[] indices;
        /// <summary>Number of initialized elements</summary>
        public int count;

        /// <summary>Constructs a sparse vector with all zeros</summary>
        /// <param name="n">Length of the vector</param>
        public SparseVector(int n)
        {
            this.n = n;
            this.items = new double[IncrementSize];
            this.indices = new int[IncrementSize];
            this.count = 0;
        }

        /// <summary>Constructs a sparse vector with defined nonzero elements</summary>
        /// <param name="items">The nonzero entries</param>
        /// <param name="indices">The locations of the nonzeros</param>
        /// <param name="n">Length of the vector</param>
        public SparseVector(double[] items, int[] indices, int n)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            if (indices == null)
                throw new ArgumentNullException("indices");
            this.items = items;
            this.indices = indices;
            this.count = items.Length;
            this.n = n;
        }

        /// <summary>Length of the sparse vector</summary>
        public int Length
        {
            get { return n; }
        }

        public SparseVector Clone()
        {
            return n == 0 ? new SparseVector() : new SparseVector((double[])items.Clone(), (int[])indices.Clone(), n);
        }

        /// <summary>Public accessor method</summary>
        /// <param name="i">Index of the request</param>
        /// <returns>The ith element of a sparse vector</returns>
        public double this[int i]
        {
            get
            {
                if (i < 0 || i >= n)
                    throw new IndexOutOfRangeException();
                int idx = Array.BinarySearch(indices, 0, count, i);
                if (idx < 0)
                    return 0;
                else
                    return items[idx];
            }
            set
            {
                if (i < 0 || i >= n)
                    throw new IndexOutOfRangeException();
                int idx = Array.BinarySearch(indices, 0, count, i);
                if (idx >= 0)
                    items[idx] = value;
                else
                {
                    int indexToAdd = ~idx;
                    if (count >= items.Length)
                    {
                        int delta = Math.Min(IncrementSize, n - items.Length);
                        int[] newIndices = new int[indices.Length + delta];
                        double[] newItems = new double[items.Length + delta];
                        Array.Copy(indices, newIndices, indices.Length);
                        Array.Copy(items, newItems, items.Length);
                        items = newItems;
                        indices = newIndices; 

                    }
                    Array.Copy(indices, indexToAdd, indices, indexToAdd + 1, count - indexToAdd);
                    Array.Copy(items, indexToAdd, items, indexToAdd + 1, count - indexToAdd);
                    count++;
                    indices[indexToAdd] = i;
                    items[indexToAdd] = value;
                }
            }
        }
    }
}
