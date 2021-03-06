﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace StardewConfigFramework {
	using DefaultOrderedDictionary = System.Collections.Specialized.OrderedDictionary;

	internal class OrderedDictionary<T>: IOrderedDictionary<T> where T : SCFObject {

		protected readonly DefaultOrderedDictionary dictionary = new DefaultOrderedDictionary();

		public event OrderedDictionaryContentsDidChange<T> ContentsDidChange;

		public T this[int index] {
			get => (T) dictionary[index];
			set {
				RemoveAt(index);
				Insert(index, value);
				ContentsDidChange?.Invoke(this);
			}
		}
		public T this[string identifier] {
			get => (T) dictionary[identifier];
			set {
				CheckIdentifierAgainstItem(identifier, value);
				dictionary[identifier] = value;
				ContentsDidChange?.Invoke(this);
			}
		}

		public bool IsReadOnly => false;

		public ICollection<string> Keys => dictionary.Keys as ICollection<string>;

		public ICollection<T> Values => dictionary.Values as ICollection<T>;

		public int Count => dictionary.Count;

		public void Add(T item) {
			dictionary.Add(item.Identifier, item);
			ContentsDidChange?.Invoke(this);
		}

		public void Add(string identifier, T item) {
			CheckIdentifierAgainstItem(identifier, item);
			dictionary.Add(identifier, item);
			ContentsDidChange?.Invoke(this);
		}

		public void Add(KeyValuePair<string, T> pair) {
			CheckKeyValuePair(pair);
			dictionary.Add(pair.Value.Identifier, pair.Value);
			ContentsDidChange?.Invoke(this);
		}

		public void Add(IList<T> items) {
			foreach (T item in items) {
				Add(item);
			}
			ContentsDidChange?.Invoke(this);
		}

		public void Clear() {
			dictionary.Clear();
			ContentsDidChange?.Invoke(this);
		}

		public bool Contains(string identifier) {
			return dictionary.Contains(identifier);
		}

		public bool Contains(T item) {
			return dictionary.Contains(item.Identifier);
		}

		public bool Contains(KeyValuePair<string, T> pair) {
			CheckKeyValuePair(pair);
			return dictionary.Contains(pair.Value.Identifier);
		}

		public bool ContainsKey(string identifier) {
			return dictionary.Contains(identifier);
		}

		public void CopyTo(T[] array, int arrayIndex) {
			int j = 0;
			for (int i = arrayIndex; i < dictionary.Count + arrayIndex; i++) {
				array[i] = (T) dictionary[j];
				j++;
			}
		}

		public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex) {
			dictionary.CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator() {
			return new SCFListEnumerator<T>(dictionary);
		}

		/// <summary>
		/// Index of the identifier
		/// </summary>
		/// <returns>The index of of the <paramref name="identifier"/>. Returns -1 if identifier does not exist.</returns>
		/// <param name="identifier">Identifier.</param>
		public int IndexOf(string identifier) {
			for (int i = 0; i < dictionary.Count; i++) {
				if (dictionary[i] == dictionary[identifier])
					return i;
			}
			return -1;
		}

		public int IndexOf(T item) {
			return IndexOf(item.Identifier);
		}

		public void Insert(int index, T item) {
			dictionary.Insert(index, item.Identifier, item);
			ContentsDidChange?.Invoke(this);
		}

		public bool Remove(T item) {
			var didRemove = Remove(item.Identifier);
			if (didRemove) {
				ContentsDidChange?.Invoke(this);
			}
			return didRemove;
		}

		public bool Remove(string identifier) {
			if (!Contains(identifier))
				return false;

			dictionary.Remove(identifier);
			ContentsDidChange?.Invoke(this);
			return true;
		}

		public bool Remove(KeyValuePair<string, T> pair) {
			CheckKeyValuePair(pair);
			var didRemove = Remove(pair.Value.Identifier);

			if (didRemove) {
				ContentsDidChange?.Invoke(this);
			}
			return didRemove;
		}

		public void RemoveAt(int index) {
			dictionary.RemoveAt(index);
			ContentsDidChange?.Invoke(this);
		}

		public bool TryGetValue(string identifier, out T value) {
			if (!Contains(identifier)) {
				value = default(T);
				return false;
			}

			value = this[identifier];
			return true;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return dictionary.GetEnumerator();
		}

		IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator() {
			return new SCFDictionaryEnumerator<T>(dictionary);
		}

		private void CheckIdentifierAgainstItem(string identifier, T item) {
			if (identifier != item.Identifier)
				throw new Exception("Identifier does not match Item's Identifier");
		}

		private void CheckKeyValuePair(KeyValuePair<string, T> pair) {
			if (pair.Key != pair.Value.Identifier)
				throw new Exception("KeyValuePair Key does not match Value's Identifier");
		}
	}
}
