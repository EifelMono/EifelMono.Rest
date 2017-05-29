using System;

namespace EifelMono.Rest
{
    /// <summary>
    /// Rest MIME type.
    /// </summary>
    public class RestMimeType : IEquatable<RestMimeType>
    {
        #region Constructore with core
        private string _AsText;
        private int _HashCode;

        /// <summary>
        /// Gets or sets as text.
        /// </summary>
        /// <value>As text.</value>
        public string AsText { get => _AsText; set => _AsText = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EifelMono.Rest.RestMimeType"/> class.
        /// </summary>
        public RestMimeType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EifelMono.Rest.RestMimeType"/> class.
        /// </summary>
        /// <param name="asText">As text.</param>
        public RestMimeType(string asText)
        {
            if (string.IsNullOrEmpty(asText))
                throw new ArgumentException();
            _AsText = asText;
        }
        #endregion

        #region Names
        /// <summary>
        /// The unknown.
        /// </summary>
        public static RestMimeType Unknown { get; private set; } = new RestMimeType("application/octet-stream");

        /// <summary>
        /// The text.
        /// </summary>
        public static RestMimeType TextPlain { get; private set; } = new RestMimeType("text/plain");

        /// <summary>
        /// The text.
        /// </summary>
        public static RestMimeType TextHtlm { get; private set; } = new RestMimeType("text/html");

        /// <summary>
        /// The json.
        /// </summary>
        public static RestMimeType ApplicationJson { get; private set; } = new RestMimeType("application/json");

        /// <summary>
        /// Gets the image png.
        /// </summary>
        /// <value>The image png.</value>
        public static RestMimeType ImagePng { get; private set; } = new RestMimeType("image/png");

        #endregion

        #region IEquatable<RestMimeType> Members

        /// <summary>
        /// Determines whether the specified <see cref="EifelMono.Rest.RestMimeType"/> is equal to the current <see cref="T:EifelMono.Rest.RestMimeType"/>.
        /// </summary>
        /// <param name="other">The <see cref="EifelMono.Rest.RestMimeType"/> to compare with the current <see cref="T:EifelMono.Rest.RestMimeType"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="EifelMono.Rest.RestMimeType"/> is equal to the current
        /// <see cref="T:EifelMono.Rest.RestMimeType"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(RestMimeType other)
        {
            if ((object)other == null)
            {
                return false;
            }
            if (object.ReferenceEquals(_AsText, other._AsText))
            {
                // Strings are static, so there is a good chance that two equal methods use the same reference
                // (unless they differ in case).
                return true;
            }
            return string.Equals(_AsText, other._AsText, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Others
        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="T:EifelMono.Rest.RestMimeType"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:EifelMono.Rest.RestMimeType"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
        /// <see cref="T:EifelMono.Rest.RestMimeType"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as RestMimeType);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="T:EifelMono.Rest.RestMimeType"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            if (_HashCode == 0)
            {
                // If _method is already uppercase, _method.GetHashCode() can be
                // used instead of _method.ToUpperInvariant().GetHashCode(),
                // avoiding the unnecessary extra string allocation.
                _HashCode = IsUpperAscii(_AsText) ? _AsText.GetHashCode() : _AsText.ToUpperInvariant().GetHashCode();
            }
            return _HashCode;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:EifelMono.Rest.RestMimeType"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:EifelMono.Rest.RestMimeType"/>.</returns>
        public override string ToString()
        {
            return _HashCode.ToString();
        }

        /// <summary>
        /// Determines whether a specified instance of <see cref="EifelMono.Rest.RestMimeType"/> is equal to another
        /// specified <see cref="EifelMono.Rest.RestMimeType"/>.
        /// </summary>
        /// <param name="left">The first <see cref="EifelMono.Rest.RestMimeType"/> to compare.</param>
        /// <param name="right">The second <see cref="EifelMono.Rest.RestMimeType"/> to compare.</param>
        /// <returns><c>true</c> if <c>left</c> and <c>right</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(RestMimeType left, RestMimeType right)
        {
            if ((object)left == null)
            {
                return ((object)right == null);
            }
            else if ((object)right == null)
            {
                return ((object)left == null);
            }
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether a specified instance of <see cref="EifelMono.Rest.RestMimeType"/> is not equal to another
        /// specified <see cref="EifelMono.Rest.RestMimeType"/>.
        /// </summary>
        /// <param name="left">The first <see cref="EifelMono.Rest.RestMimeType"/> to compare.</param>
        /// <param name="right">The second <see cref="EifelMono.Rest.RestMimeType"/> to compare.</param>
        /// <returns><c>true</c> if <c>left</c> and <c>right</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(RestMimeType left, RestMimeType right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Ises the upper ASCII.
        /// </summary>
        /// <returns><c>true</c>, if upper ASCII was ised, <c>false</c> otherwise.</returns>
        /// <param name="value">Value.</param>
        private static bool IsUpperAscii(string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (!(c >= 'A' && c <= 'Z'))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }

}

