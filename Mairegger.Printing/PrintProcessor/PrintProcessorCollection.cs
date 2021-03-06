﻿// Copyright 2015 Michael Mairegger
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Mairegger.Printing.PrintProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Printing;
    using System.Windows.Controls;

    public class PrintProcessorCollection : Collection<PrintProcessor>, IPrintProcessorPrinter
    {
        public PrintProcessorCollection(PrintProcessor printProcessor)
            : this(new List<PrintProcessor> { printProcessor }, printProcessor.FileName)
        {
            if (printProcessor == null)
            {
                throw new ArgumentNullException(nameof(printProcessor));
            }
        }

        public PrintProcessorCollection(IEnumerable<PrintProcessor> coll, string fileName = "")
            : this(new List<PrintProcessor>(coll), fileName)
        {
        }

        public PrintProcessorCollection(IList<PrintProcessor> coll, string fileName = "")
        {
            if (coll == null)
            {
                throw new ArgumentNullException(nameof(coll));
            }

            FileName = fileName;
            foreach (var printProcessor in coll)
            {
                if (printProcessor != null)
                {
                    Add(printProcessor);
                }
            }
        }

        public string FileName { get; private set; }

        /// <summary>
        ///     Sets whether for each <see cref="System.Printing.PrintProcessor" /> in <see cref="Collection{T}.Items" /> the page
        ///     numbers begins with 0.
        /// </summary>
        public bool IndividualPageNumbers { get; set; }

        public virtual void OnPageBreak()
        {
        }

        /// <summary>
        ///     Creates the document in order to provide a preview of the document
        /// </summary>
        public void PreviewDocument()
        {
            PrintProcessor.PreviewDocument(this);
        }

        /// <summary>
        ///     Prints the document.
        /// </summary>
        /// <returns> True if succeeds, false otherwise, or if the use cancels the print process. </returns>
        public bool PrintDocument()
        {
            if (Count == 0)
            {
                return false;
            }

            var p = this.First();
            PrintDialog pd = p.PrintDialog;

            if (pd.ShowDialog().Equals(false))
            {
                return false;
            }

            return PrintProcessor.PrintDocument(pd, this);
        }

        /// <summary>
        ///     Prints the document to the given <see cref="PrintServer" /> and the given <see cref="PrintQueue.Name" />
        /// </summary>
        /// <param name="printQueueName"> The name of the print queue. </param>
        /// <param name="printServer"> The print server to host the print queue. </param>
        /// ///
        /// <returns> True if succeeds, false otherwise, or if the use cancels the print process. </returns>
        public bool PrintDocument(string printQueueName, PrintServer printServer)
        {
            if (Count == 0)
            {
                return false;
            }

            var p = this.First();
            PrintDialog pd = p.PrintDialog;
            pd.PrintQueue = new PrintQueue(printServer, printQueueName);

            return PrintProcessor.PrintDocument(pd, this);
        }

        /// <summary>
        ///     Prints the document to a <see cref="LocalPrintServer" /> and the given <see cref="PrintQueue.Name" /> of the print
        ///     queue.
        /// </summary>
        /// <param name="printQueueName"> The Print-server to print on. </param>
        /// <returns> True if succeeds, false otherwise, or if the use cancels the print process. </returns>
        public bool PrintDocument(string printQueueName)
        {
            using (var printServer = new LocalPrintServer())
            {
                return PrintDocument(printQueueName, printServer);
            }
        }
    }
}