/*
 * Copyright (c) 2016 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

namespace Tizen.Multimedia.Vision
{
    /// <summary>
    /// Represents a configuration for the image to be generated by <see cref="BarcodeGenerator"/>.
    /// </summary>
    /// <since_tizen> 3 </since_tizen>
    public class BarcodeImageConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeImageConfiguration"/> class.
        /// </summary>
        /// <remarks>
        /// The mediastorage privilege (http://tizen.org/privilege/mediastorage) is needed if the image path is relevant to media storage.\n
        /// The externalstorage privilege (http://tizen.org/privilege/externalstorage) is needed if the image path is relevant to external storage.
        /// </remarks>
        /// <param name="size">The <see cref="Size"/> of the generated image.</param>
        /// <param name="path">The path to the file to be generated.</param>
        /// <param name="imageFormat">The format of the output image.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The width of <paramref name="size"/> is less than or equal to zero.\n
        ///     -or-\n
        ///     The height of <paramref name="size"/> is less than or equal to zero.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="imageFormat"/> is invalid.</exception>
        /// <code>
        /// BarcodeImageConfiguration imageConfig = new BarcodeImageConfiguration(new Size(500, 400), "/opt/usr/test-barcode-generate-new", BarcodeImageFormat.JPG);
        /// </code>
        /// <since_tizen> 3 </since_tizen>
        public BarcodeImageConfiguration(Size size, string path, BarcodeImageFormat imageFormat)
        {
            if (size.Width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Size.Width), size.Width,
                    "width can't be less than or equal to zero.");
            }

            if (size.Height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Size.Height), size.Height,
                    "height can't be less than or equal to zero.");
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            ValidationUtil.ValidateEnum(typeof(BarcodeImageFormat), imageFormat);

            Size = size;
            Path = path;
            Format = imageFormat;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeImageConfiguration"/> class.
        /// </summary>
        /// <remarks>
        /// The mediastorage privilege (http://tizen.org/privilege/mediastorage) is needed if the image path is relevant to media storage.\n
        /// The externalstorage privilege (http://tizen.org/privilege/externalstorage) is needed if the image path is relevant to external storage.
        /// </remarks>
        /// <param name="width">The width of the image to be generated.</param>
        /// <param name="height">The height of the image to be generated.</param>
        /// <param name="path">The path to the file to be generated.</param>
        /// <param name="imageFormat">The format of the output image.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="width"/> is less than or equal to zero.\n
        ///     -or-\n
        ///     <paramref name="height"/> is less than or equal to zero.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="imageFormat"/> is invalid.</exception>
        /// <code>
        /// BarcodeImageConfiguration imageConfig = new BarcodeImageConfiguration(500, 400, "/opt/usr/test-barcode-generate-new", BarcodeImageFormat.JPG);
        /// </code>
        /// <since_tizen> 3 </since_tizen>
        public BarcodeImageConfiguration(int width, int height, string path, BarcodeImageFormat imageFormat)
            : this(new Size(width, height), path, imageFormat)
        {
        }

        /// <summary>
        /// Gets the size of the image.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public Size Size { get; }

        /// <summary>
        /// Gets the width of the image.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public int Width => Size.Width;

        /// <summary>
        /// Gets the height of the image.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public int Height => Size.Height;

        /// <summary>
        /// Gets the path to the file that has to be generated.
        /// </summary>
        /// <remarks>
        /// The mediastorage privilege (http://tizen.org/privilege/mediastorage) is needed if the image path is relevant to media storage.\n
        /// The externalstorage privilege (http://tizen.org/privilege/externalstorage) is needed if the image path is relevant to external storage.
        /// </remarks>
        /// <since_tizen> 3 </since_tizen>
        public string Path { get; }

        /// <summary>
        /// Gets the format of the output image.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public BarcodeImageFormat Format { get; }
    }
}
