namespace NeptunLight.Services
{
    /// <summary>
    ///     A key-value storage that persists its content across application / OS restarts.
    /// </summary>
    public interface IPrimitiveStorage
    {
        /// <summary>
        ///     Clears the entry with the given key from the storage.
        ///     Noop but does not throw if no such entry was found.
        /// </summary>
        void ClearValue(string key);

        /// <summary>
        ///     Checks if a given key exists in the storage dictionary.
        /// </summary>
        bool ContainsKey(string key);

        /// <summary>
        ///     Retrives a string value from storage.
        /// </summary>
        /// <param name="key">The key of the stored value.</param>
        /// <returns>The stored value.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown if no value was saved using the given key.</exception>
        string GetString(string key);

        /// <summary>
        ///     Places a string into the persistent storage using the given key.
        ///     Overrides existing value with same key.
        /// </summary>
        void PutString(string key, string value);

        /// <summary>
        ///     Retrives an integer value from storage.
        /// </summary>
        /// <param name="key">The key of the stored value.</param>
        /// <returns>The stored value.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown if no value was saved using the given key.</exception>
        int GetInt(string key);

        /// <summary>
        ///     Places an integer into the persistent storage using the given key.
        ///     Overrides existing value with same key.
        /// </summary>
        void PutInt(string key, int value);

        /// <summary>
        ///     Retrives a double value from storage.
        /// </summary>
        /// <param name="key">The key of the stored value.</param>
        /// <returns>The stored value.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown if no value was saved using the given key.</exception>
        double GetDouble(string key);

        /// <summary>
        ///     Places a double value into the persistent storage using the given key.
        ///     Overrides existing value with same key.
        /// </summary>
        void PutDouble(string key, double value);
    }
}