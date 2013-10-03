using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;

namespace Wedo.Utility.Reflection
{
    #region Locker

    class LockWrite : IDisposable
    {

        ReaderWriterLockSlim readerwriterlock;

        public LockWrite(ReaderWriterLockSlim readerwriterlock)
        {

            this.readerwriterlock = readerwriterlock;

            readerwriterlock.TryEnterWriteLock(10000);

        }



        public void Dispose()
        {

            readerwriterlock.ExitWriteLock();

        }

    }

    class LockRead : IDisposable
    {

        ReaderWriterLockSlim readerwriterlock;

        public LockRead(ReaderWriterLockSlim readerwriterlock)
        {

            this.readerwriterlock = readerwriterlock;

            readerwriterlock.EnterReadLock();

        }



        public void Dispose()
        {

            readerwriterlock.ExitReadLock();

        }

    }

    #endregion



    #region SyncDictionary

    [Serializable]

    public class SyncDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {

        Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();



        ReaderWriterLockSlim synclock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);



        public void Add(TKey key, TValue value)
        {

            using (new LockWrite(synclock))
            {

                dict.Add(key, value);

            }

        }



        public bool ContainsKey(TKey key)
        {

            using (new LockRead(synclock))
            {

                return dict.ContainsKey(key);

            }

        }



        public ICollection<TKey> Keys
        {

            get
            {

                using (new LockRead(synclock))
                {

                    return dict.Keys.ToList().AsReadOnly();

                }

            }

        }



        public bool Remove(TKey key)
        {

            using (new LockWrite(synclock))
            {

                return dict.Remove(key);

            }

        }



        public bool TryGetValue(TKey key, out TValue value)
        {

            using (new LockRead(synclock))
            {

                return dict.TryGetValue(key, out value);

            }

        }



        public ICollection<TValue> Values
        {

            get
            {

                using (new LockRead(synclock))
                {

                    return dict.Values.ToList().AsReadOnly();

                }

            }

        }



        public TValue this[TKey key]
        {

            get
            {

                using (new LockRead(synclock))
                {

                    return dict[key];

                }

            }

            set
            {

                using (new LockWrite(synclock))
                {

                    dict[key] = value;

                }

            }

        }



        public void Add(KeyValuePair<TKey, TValue> item)
        {

            Add(item.Key, item.Value);

        }



        public void Clear()
        {

            using (new LockWrite(synclock))
            {

                dict.Clear();

            }

        }



        public bool Contains(KeyValuePair<TKey, TValue> item)
        {

            return ContainsKey(item.Key);

        }



        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {

            throw new NotImplementedException();

        }



        public int Count
        {

            get { using (new LockRead(synclock)) { return dict.Count; } }

        }



        public bool IsReadOnly
        {

            get { return false; }

        }



        public bool Remove(KeyValuePair<TKey, TValue> item)
        {

            return Remove(item.Key);

        }



        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {

            using (new LockRead(synclock))
            {

                return dict.GetEnumerator();

            }

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    #endregion

    #region SyncList
    /// <summary>
    /// 同步List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SyncList<T> : IList<T>
    {

        List<T> value = new List<T>();

        ReaderWriterLockSlim synclock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public int IndexOf(T item)
        {
            using (new LockRead(synclock))
            {
                return value.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            using (new LockWrite(synclock))
            {
                value.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            using (new LockWrite(synclock))
            {
                value.RemoveAt(index);
            }
        }

        public T this[int index]
        {

            get
            {
                using (new LockRead(synclock))
                {
                    return value[index];
                }
            }

            set
            {
                using (new LockWrite(synclock))
                {
                    this.value[index] = value;
                }
            }
        }



        public void Add(T item)
        {
            using (new LockWrite(synclock))
            {
                value.Add(item);
            }
        }

        public void Clear()
        {
            using (new LockWrite(synclock))
                value.Clear();
        }

        public bool Contains(T item)
        {
            using (new LockRead(synclock))
                return value.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            using (new LockRead(synclock))
                value.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                using (new LockRead(synclock))
                    return value.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            using (new LockWrite(synclock))
                return value.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            using (new LockRead(synclock))
                return value.GetEnumerator();
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

    #endregion
}
