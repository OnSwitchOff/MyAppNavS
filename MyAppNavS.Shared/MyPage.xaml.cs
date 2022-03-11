
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

using Microsoft.UI.Xaml.Controls;
using PDFCreator;
using PDFCreator.Enums;
using PDFCreator.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace MyAppNavS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyPage : Page
    {
        public MyPage()
        {
            this.InitializeComponent();
            btn.Click += Btn_Click;
        }

        private void Btn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            PartnerModel partner = new PartnerModel();
            partner.Company = "ММБ 7 ЕООД";
            partner.Address = "София - ж.к. Дружба бл. 221 вх. Б эт. 4 ап. 37";
            partner.Principal = "Мария Михайлова Борисова";
            partner.TaxNumber = "203456185";
            partner.Phone = "0896712690";
            partner.VATNumber = "";

            DocumentFactory documentFactory = new DocumentFactory();
            documentFactory.CustomerData = partner;

            //documentFactory.DocumentDescription.DocumentName = "Фактура";
            documentFactory.DocumentDescription.DocumentDescription = "към фактура";
            documentFactory.DocumentDescription.DocumentNumber = "0000120053";
            documentFactory.DocumentDescription.SourceDocumentNumber = "0000000008";
            documentFactory.DocumentDescription.SourceDocumentDate = new DateTime(2021, 3, 29);
            //documentFactory.DocumentDescription.DocumentDate = new DateTime(2021, 2, 5);
            //documentFactory.DocumentDescription.DocumentAuthenticity = DocumentAuthenticity.Original;
            //documentFactory.DocumentDescription.PaymentType = "Превод по сметка";
            documentFactory.DocumentDescription.DocumentSum = "Сто и три лв. и 43 ст.";
            //documentFactory.DocumentDescription.DealReason = "Продажба";
            documentFactory.DocumentDescription.DealPlace = "Центр. офис София";
            documentFactory.DocumentDescription.DealDescription = "";
            documentFactory.DocumentDescription.TaxDate = new DateTime(2021, 2, 5);
            documentFactory.DocumentDescription.CreatedBy = "Стефка Стойчева";
            documentFactory.DocumentDescription.ReceivedBy = "Мария Михайлова Борисова";

            documentFactory.GoodsTable.Columns.Add("Кол.", typeof(double));
            documentFactory.GoodsTable.Columns.Add("Мярка", typeof(string));
            documentFactory.GoodsTable.Columns.Add("Описание на стоката или услугата", typeof(string));
            documentFactory.GoodsTable.Columns.Add("Цена", typeof(double));
            documentFactory.GoodsTable.Columns.Add("Стойност", typeof(double));
            for (int i = 1; i < 39; i++)
            {
                documentFactory.GoodsTable.Rows.Add(i, "бр.", "Актуализация на програмен продукт - 1 бр. актуализационен код", 86.19, 86.19);
            }

            documentFactory.ReportTable.Columns.Add("Col1", typeof(int));
            documentFactory.ReportTable.Columns.Add("Col2", typeof(string));
            documentFactory.ReportTable.Columns.Add("Col3", typeof(double));
            documentFactory.ReportTable.Columns.Add("Col4", typeof(DateTime));
            Random random = new Random();

            for (int i = 0; i < 20; i++)
            {
                documentFactory.ReportTable.Rows.Add(i + 1, string.Format("Item{0}", i + 1), random.NextDouble() * 1000, DateTime.Now);
            }

            documentFactory.GenerateDocument(DocumentType.Report, DocumentVersionPrinting.OriginalAndTwoCopies, PaymentTypes.Cash);

            documentFactory.SaveDocument(System.IO.Path.Combine(@"E:\UnoProjects\MyAppNavS", "NewPdf.pdf"));

        }



    }

    public class DocumentFactory
    {
        #region "Declarations"
        // класс, генерирующий pdf документ
        private MicroinvestPdfDocument pdfDocument;
        // параметры страницы
        private PrintParametersModel pageParameters;
        // высота верхнего колонтитула первой страницы
        private double firstPageHeaderHeight;
        // путь к изображению с логотипом компании
        private string logoPath;
        // путь к изображению с реквизитами компании
        private string signaturePath;
        // изображение с логотипом компании
        private System.Drawing.Image logo;
        // изображение с реквизитами компании
        private System.Drawing.Image signature;
        // таблица с данными отчёта
        private DataTable reportTable;
        // ширина колонок таблицы отчётов
        private double[] reportTableColumnsWidth;
        // ширина колонок таблицы товаров в печатных формах
        private double[] goodsTableColumnsWidth;
        #endregion

        #region "Constructor"
        /// <summary>
        /// Создаёт экземпляр класса MicroinvestPdfDocument и 
        /// устанавливает ему флаг использования на первой странице отличного от остальных верхнего и нижнего колонтитулов.
        /// Устанавливает логотип и реквизиты компании по умолчанию
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>19.04.2021</date>
        public DocumentFactory()
        {
            // создаём экземпляр класса MicroinvestPdfDocument, который будет генерировать Pdf документ
            pdfDocument = new MicroinvestPdfDocument();
            // устанавливаем флаг, что верхний и нижний колонтитулы первой страницы будут отлисными от остальных страниц
            pdfDocument.DifferentFirstPageHeaderFooter = true;
            // устанавливаем кол-во знаков после запятой при указании цены !!!взять из CurrentCultureHelper
            pdfDocument.PriceFormat = 2;
            // устанавливаем кол-во знаков после запятой при указании количества !!!взять из CurrentCultureHelper
            pdfDocument.QuantityFormat = 3;

            // запоминаем высоту верхнего колонтитула первой страницы, чтобы иметь возможность восстановить её в дальнейшем
            firstPageHeaderHeight = pdfDocument.FirstPageHeaderHeight;


            // устанавливаем путь к изображению с логотипом фирмы !!!!!! получить путь к изображению из базы данных !!!!!!!!!!!!!!!
            logoPath = string.Empty;
            // устанавливаем путь к изображению с реквизитами фирмы !!!!!! получить путь к изображению из базы данных !!!!!!!!!!!!!!!
            signaturePath = string.Empty;
            // устанавливаем логотип и изображение с реквизитами компании по умолчанию
            logo = pdfDocument.DefaultHeaderImage;
            signature = pdfDocument.DefaultFooterImage;

            // устанавливаем данные по нашей компании !!!! грузим данные из базы !!!!!!!!!!!!!!!!
            pdfDocument.Saler.Name = "Микроинвест ООД";
            pdfDocument.Saler.Address = "София бул. Цар Борис III 215";
            pdfDocument.Saler.Principal = "Елена Ширкова";
            pdfDocument.Saler.TaxNumber = "831826092";
            pdfDocument.Saler.VATNumber = "BG831826092";
            pdfDocument.Saler.BankName = "ПроКредит Банк България ЕАД";
            pdfDocument.Saler.BIC = "PRCBBGSF";
            pdfDocument.Saler.IBAN = "BG12PRCB92301015430322";
            pdfDocument.Saler.StoreName = "Служебен обект";
            // устанавливаем данные по группам НДС !!!! грузим данные из базы !!!!!!!!!!!!!!!
            pdfDocument.VATs.Add(new VATModel() { VATRate = 20, VATBase = 86.19, VATSum = 17.24 });
            pdfDocument.VATs.Add(new VATModel() { VATRate = 9, VATBase = 86.19, VATSum = 17.24 });
            pdfDocument.VATs.Add(new VATModel() { VATRate = 10, VATBase = 86.19, VATSum = 17.24 });
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// Таблица с данными, необходимыми для построения отчёта
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>19.04.2021</date>
        public DataTable ReportTable
        {
            get => reportTable == null ? reportTable = new DataTable() : reportTable;
            set => reportTable = value;
        }

        /// <summary>
        /// Ширина колонок таблицы с данными, необходимыми для построения отчёта
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>19.04.2021</date>
        public double[] ReportTableColumnsWidth
        {
            get
            {
                // если массив с шириной колонок таблицы не сформирован - формируем его с данными по умолчанию
                if (reportTableColumnsWidth == null)
                {
                    reportTableColumnsWidth = new double[ReportTable.Columns.Count > 0 ? ReportTable.Columns.Count : 1];
                    double defaultWidth = (pdfDocument.PageWidth - pdfDocument.LeftIndentation - pdfDocument.RightIndentation) / reportTableColumnsWidth.Length;
                    for (int i = 0; i < reportTableColumnsWidth.Length; i++)
                    {
                        reportTableColumnsWidth[i] = defaultWidth;
                    }
                }

                return reportTableColumnsWidth;
            }
            set => reportTableColumnsWidth = value;
        }


        /// <summary>
        /// Таблицы с данными о товарах, участвующих в документе
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>19.04.2021</date>
        public DataTable GoodsTable
        {
            get => pdfDocument.DocumentData == null ? pdfDocument.DocumentData = new DataTable() : pdfDocument.DocumentData;
            set => pdfDocument.DocumentData = value;
        }

        /// <summary>
        /// Ширина колонок таблицы с данными о товарах
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>19.04.2021</date>
        public double[] GoodsTableColumnsWidth
        {
            get
            {
                // если массив с шириной колонок таблицы не сформирован - формируем его с данными по умолчанию
                if (goodsTableColumnsWidth == null)
                {
                    goodsTableColumnsWidth = new double[GoodsTable.Columns.Count > 0 ? GoodsTable.Columns.Count : 1];
                    double defaultWidth = (pdfDocument.PageWidth - pdfDocument.LeftIndentation - pdfDocument.RightIndentation) / goodsTableColumnsWidth.Length;
                    for (int i = 0; i < goodsTableColumnsWidth.Length; i++)
                    {
                        goodsTableColumnsWidth[i] = defaultWidth;
                    }
                    //GoodsTableColumnsWidth = new double[] { 1.5, 1.5, (pdfDocument.PageWidth - pdfDocument.LeftIndentation - pdfDocument.RightIndentation - 7), 2, 2 };
                }

                return goodsTableColumnsWidth;
            }
            set => goodsTableColumnsWidth = value;
        }

        /// <summary>
        /// Путь к изображению с логотипом фирмы
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>19.04.2021</date>
        public string LogoPath
        {
            get => logoPath;
            set => SetImageValues(ref logoPath, value, ref logo);
        }

        /// <summary>
        /// Путь к изображению с реквизитами фирмы
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>19.04.2021</date>
        public string SignaturePath
        {
            get => signaturePath;
            set => SetImageValues(ref signaturePath, value, ref signature);
        }

        /// <summary>
        /// Информация о покупателе
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>19.04.2021</date>
        public PartnerModel CustomerData
        {
            set => pdfDocument.Client = (PDFCreator.Models.CompanyModel)value;
        }

        /// <summary>
        /// Информация о формируемом документе
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>19.04.2021</date>
        public DocumentModel DocumentDescription
        {
            get => pdfDocument.Document;
            set => pdfDocument.Document = value;
        }

        /// <summary>
        /// Информация о параметрах страницы документа
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>19.04.2021</date>
        public PrintParametersModel PageParameters
        {
            get => pageParameters == null ? pageParameters = new PrintParametersModel() : pageParameters;
            set
            {
                pageParameters = value;

                pdfDocument.LeftIndentation = pageParameters.LeftMargin;
                pdfDocument.TopIndentation = pageParameters.TopMargin;
                pdfDocument.RightIndentation = pageParameters.RightMargin;
                pdfDocument.BottomIndentation = pageParameters.BottomMargin;
                pdfDocument.Orientation = (PageOrientation)Enum.Parse(typeof(PageOrientation), pageParameters.SelectedPageOrientation.Value.ToString());
                pdfDocument.PageFormat = (PageFormat)Enum.Parse(typeof(PageFormat), pageParameters.SelectedPageFormat.Value.ToString());
            }
        }
        #endregion

        #region "Public methods"
        /// <summary>
        /// Сгенерировать разметку документа
        /// </summary>
        /// <param name="documentType">Тип документа</param>
        /// <param name="versionPrinting">Версия документа для печати (оригинал, копия и т.п.)</param>
        /// <param name="paymentTypes">Тип оплаты по текущему документу</param>
        /// <param name="operationType">Тип операции</param>
        /// <returns>true - если разметка удачно сгенерирована, иначе false</returns>
        /// <developer>Сергей Рознюк</developer>
        /// <date>19.04.2021</date>
        [Obsolete]
        public bool GenerateDocument(DocumentType documentType, DocumentVersionPrinting versionPrinting = DocumentVersionPrinting.Original, PaymentTypes paymentTypes = PaymentTypes.Cash, OperationType operationType = OperationType.Sale)
        {
            // очищаем предыдущую разметку (если таковая осталась с предыдущего раза) 
            pdfDocument.Clear();
            try
            {
                // если необходимо разметить отчёт
                if (documentType == DocumentType.Report)
                {
                    // обнуляем "задел", выделенный для верхнего колонтитула первой страницы
                    pdfDocument.FirstPageHeaderHeight = 0;
                    // устанавливаем стиль для визуализации таблицы
                    TableVisualizationModel tableVisualization = new TableVisualizationModel();
                    tableVisualization.HeaderBackground.Color_R = 30;
                    tableVisualization.HeaderBackground.Color_G = 144;
                    tableVisualization.HeaderBackground.Color_B = 255;
                    tableVisualization.HeaderFont.IsBold = true;
                    tableVisualization.DifferentEvenRowsBackground = true;
                    // формируем разметку таблицы
                    pdfDocument.AddNewItemToContent(pdfDocument.CreateAndFillTable(reportTable, ReportTableColumnsWidth, tableVisualization));
                }
                else // иначе
                {
                    // восстанавливаем высоту "задела", выделенного для верхнего колонтитула первой страницы
                    pdfDocument.FirstPageHeaderHeight = firstPageHeaderHeight;
                    // размечаем верхний и нижний колонтитулы
                    pdfDocument.CreateDefaultHeaderFooterGrid();
                    // добавляем в ячейку 0Х0 верхнего колонтитула первой страницы изображение с логотипом нашей фирмы
                    pdfDocument.AddNewItemToFirsPageHeaderFooterGrid(
                        DocumentArea.Header,
                        pdfDocument.CreateImageObject(
                            logo, pdfDocument.GetColumnWidth(pdfDocument.FirstPageHeaderGrid, 0),
                            HorizontalAlignment.Left,
                            pdfDocument.FirstPageHeaderHeight),
                        0,
                        0);
                    // добавляем в ячейку 0х1 верхнего колонтитула первой страницы реквизиты нашей фирмы
                    pdfDocument.AddNewItemToFirsPageHeaderFooterGrid(
                        DocumentArea.Header,
                        pdfDocument.PrepareOurCompanyData(
                            pdfDocument.GetColumnWidth(pdfDocument.FirstPageHeaderGrid, 1) / 3,
                            pdfDocument.GetColumnWidth(pdfDocument.FirstPageHeaderGrid, 1) / 3 * 2),
                        0,
                        1);
                    // добавляем в ячейку 0х1 нижнего колонтитула первой страницы изображение 
                    pdfDocument.AddNewItemToFirsPageHeaderFooterGrid(
                        DocumentArea.Footer,
                        pdfDocument.CreateImageObject(
                            signature, pdfDocument.GetColumnWidth(pdfDocument.FirstPageFooterGrid, 1),
                            HorizontalAlignment.Right,
                            pdfDocument.FooterHeight),
                        0,
                        1);
                    // в ячейке 0х0 нижнего колонтитула будет находиться нумерация строк (вставлена при выполнении функции "CreateDefaultHeaderFooterGrid")
                    // добавляем в ячейку 0х1 нижнего колонтитула изображение 
                    pdfDocument.AddNewItemToHeaderFooterGrid(
                        DocumentArea.Footer,
                        pdfDocument.CreateImageObject(
                            signature, pdfDocument.GetColumnWidth(pdfDocument.FirstPageFooterGrid, 1),
                            HorizontalAlignment.Right,
                            pdfDocument.FooterHeight),
                        0,
                        1);
                    // формируем тело документа в зависимости от выбранных пользователем параметров
                    switch (versionPrinting)
                    {
                        case DocumentVersionPrinting.Original:
                            GenerateDocumentBody(DocumentAuthenticity.Original, documentType, operationType, paymentTypes);
                            break;
                        case DocumentVersionPrinting.Copy:
                            GenerateDocumentBody(DocumentAuthenticity.Copy, documentType, operationType, paymentTypes);
                            break;
                        case DocumentVersionPrinting.OriginalAndCopy:
                            // формируем оригинал документа
                            GenerateDocumentBody(DocumentAuthenticity.Original, documentType, operationType, paymentTypes);
                            // добавляем секцию для следующего экземпляра документа
                            pdfDocument.AddSection();
                            // формируем копию документа
                            GenerateDocumentBody(DocumentAuthenticity.Copy, documentType, operationType, paymentTypes);
                            break;
                        case DocumentVersionPrinting.OriginalAndTwoCopies:
                            GenerateDocumentBody(DocumentAuthenticity.Original, documentType, operationType, paymentTypes);
                            pdfDocument.AddSection();
                            GenerateDocumentBody(DocumentAuthenticity.Copy, documentType, operationType, paymentTypes);
                            pdfDocument.AddSection();
                            GenerateDocumentBody(DocumentAuthenticity.Copy, documentType, operationType, paymentTypes);
                            break;
                        default:
                            break;
                    }
                }

                // генерируем разметку
                pdfDocument.RenderDocument();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Сохранить документ по указанному пути
        /// </summary>
        /// <param name="path">Путь, по которому будет сохранён документ</param>
        /// <returns>true - если документ сохранён, иначе false</returns>
        /// <developer>Сергей Рознюк</developer>
        /// <date>19.04.2021</date>
        [Obsolete]
        public bool SaveDocument(string path)
        {
            // если документ размечен - сохраняем его и возвращаем true
            if (pdfDocument.IsRenderedDocument)
            {
                pdfDocument.Save(path);

                return true;
            }

            // иначе возвращаем false
            return false;
        }

        /// <summary>
        /// Преобразовать страницы документа в изображения
        /// </summary>
        /// <returns>Коллекция страниц документа</returns>
        /// <developer>Сергей Рознюк</developer>
        /// <date>19.04.2021</date>
        [Obsolete]
        public List<System.Drawing.Image> ConvertDocumentToImage()
        {
            // если документ размечен - возвращаем коллекцию с изображениями страниц
            if (pdfDocument.IsRenderedDocument)
            {
                return pdfDocument.ConvertPdfToImage();
            }

            // иначе возвращаем пустую коллекцию
            return new List<System.Drawing.Image>();
        }
        #endregion

        #region "Private methods"
        /// <summary>
        /// Сгенерировать тело документа
        /// </summary>
        /// <param name="documentType">Тип документа</param>
        /// <param name="documentAuthenticity">Версия документа (оригинал или копия)</param>
        /// <param name="paymentTypes">Тип оплаты по текущему документу</param>
        /// <param name="operationType">Тип операции</param>
        /// <developer>Сергей Рознюк</developer>
        /// <date>19.04.2021</date>
        [Obsolete]
        private void GenerateDocumentBody(DocumentAuthenticity documentAuthenticity, DocumentType documentType, OperationType operationType, PaymentTypes paymentTypes)
        {
            // создаём и размечаем блок с данными о клиенте и документе
            pdfDocument.CreateCaptionSection();
            // добаляем данный блок в тело документа
            pdfDocument.AddNewItemToContent(pdfDocument.CaptionSection);
            // добавляем отступ
            pdfDocument.AddNewItemToContent(pdfDocument.CreateSpace(1.3));
            // добавляем в тело документа таблицу с товарами
            pdfDocument.AddNewItemToContent(pdfDocument.PrepareOperationData(GoodsTableColumnsWidth));
            // добавляем в тело документа таблицу с разбивкой по таварам в зависимости от групп НДС
            pdfDocument.AddNewItemToContent(pdfDocument.PrepareVATData(4, 2));
            // создаём и размечаем блок с информацией об оплате и описании документа
            pdfDocument.CreateAdditionalDataSection();
            // добавляем в тело данный блок 
            pdfDocument.AddNewItemToContent(pdfDocument.AdditionalDataSection);
            // добавляем в тело документа информацию о том, кто составил данный документ и кто его получит 
            pdfDocument.AddNewItemToContent(pdfDocument.PrepareSignatureData());


            pdfDocument.Document.DocumentName = documentType.ToString(); // взять со словаря!!!
            pdfDocument.Document.DocumentDate = DateTime.Now;
            pdfDocument.Document.DocumentAuthenticity = documentAuthenticity;
            pdfDocument.Document.PaymentType = paymentTypes.ToString(); // взять со словаря!!!
            pdfDocument.Document.DealReason = operationType.ToString(); // взять со словаря!!!

            switch (documentType)
            {
                case DocumentType.Invoice:
                    pdfDocument.Document.DocumentDescription = "";
                    pdfDocument.Document.SourceDocumentNumber = "";
                    pdfDocument.Document.SourceDocumentDate = DateTime.Now;
                    break;
                case DocumentType.DebitNote:
                    break;
                case DocumentType.CreditNote:
                    break;
                case DocumentType.ProformInvoice:
                    pdfDocument.Document.DocumentDescription = "";
                    pdfDocument.Document.SourceDocumentNumber = "";
                    pdfDocument.Document.SourceDocumentDate = DateTime.Now;
                    break;
                case DocumentType.Receipt:
                    pdfDocument.Document.DocumentDescription = "за продажба на стоки"; // взять со словаря в зависимости от типа операции!!!
                    pdfDocument.Document.SourceDocumentNumber = "";
                    pdfDocument.Document.SourceDocumentDate = DateTime.Now;

                    pdfDocument.Document.DocumentAuthenticity = DocumentAuthenticity.Unknown;
                    break;
                default:
                    break;
            }


            // вставляем в ячейку 0Х0 CaptionSection информацию о клиенте
            pdfDocument.AddNewItemToCaptionSection(
                pdfDocument.PrepareClientData(
                    pdfDocument.GetColumnWidth(
                        pdfDocument.CaptionSection, 0) / 4,
                    pdfDocument.GetColumnWidth(
                        pdfDocument.CaptionSection, 0) / 4 * 3
                    ),
                0,
                0);
            // вставляем в ячейку 0Х1 CaptionSection информацию о документе
            pdfDocument.AddNewItemToCaptionSection(
                pdfDocument.PrepareDocumentData(
                    pdfDocument.GetColumnWidth(
                        pdfDocument.CaptionSection, 1) / 2,
                    pdfDocument.GetColumnWidth(
                        pdfDocument.CaptionSection, 1) / 2
                    ),
                0,
                1);

            // вставляем в ячейку 0Х0 AdditionalDataSection информацию об оплате
            pdfDocument.AddNewItemToAdditionalDataSection(
                pdfDocument.PreparePaymentData(
                    pdfDocument.GetColumnWidth(pdfDocument.AdditionalDataSection, 0) / 2,
                    pdfDocument.GetColumnWidth(pdfDocument.AdditionalDataSection, 0) / 2),
                0,
                0
                );
            // если данный документ не является квитанцией
            if (documentType != DocumentType.Receipt)
            {
                // вставляем в ячейку 1Х0 AdditionalDataSection информацию о банке
                pdfDocument.AddNewItemToAdditionalDataSection(
                    pdfDocument.PrepareBankData(
                        pdfDocument.GetColumnWidth(pdfDocument.AdditionalDataSection, 0) / 4,
                        pdfDocument.GetColumnWidth(pdfDocument.AdditionalDataSection, 0) / 4 * 3),
                    1,
                    0
                    );
                // вставляем в ячейку 1Х1 AdditionalDataSection описание по текущей сделке 
                pdfDocument.AddNewItemToAdditionalDataSection(
                    pdfDocument.PrepareDealDescriptionData(
                        pdfDocument.GetColumnWidth(pdfDocument.AdditionalDataSection, 1) / 2,
                        pdfDocument.GetColumnWidth(pdfDocument.AdditionalDataSection, 1) / 2),
                    1,
                    1
                    );
            }
        }

        /// <summary>
        /// Установить значения в поля, отвечающие за изображения (напр., логотип, реквизиты и т.п.)
        /// </summary>
        /// <param name="property">Поле, в которое необходимо записать путь к изображению</param>
        /// <param name="imagePath">Путь к изображению</param>
        /// <param name="image">Объект с изображение</param>
        /// <developer>Сергей Рознюк</developer>
        /// <date>19.04.2021</date>
        private void SetImageValues(ref string property, string imagePath, ref System.Drawing.Image image)
        {
            // если файл существует
            if (System.IO.File.Exists(imagePath))
            {
                // получаем разрешение файла
                int lastPointIndex = imagePath.LastIndexOf('.');
                string fileFormat = string.Empty;
                if (lastPointIndex > 0)
                {
                    fileFormat = imagePath.Substring(lastPointIndex + 1, imagePath.Length - lastPointIndex).ToLower();
                }

                // если данный файл является изображением - сохраняем параметры в соответствующих полях
                if (!string.IsNullOrEmpty(fileFormat) && (fileFormat.Equals("png") || fileFormat.Equals("jpg") || fileFormat.Equals("jpeg") || fileFormat.Equals("bmp")))
                {
                    property = imagePath;
                    image = System.Drawing.Image.FromFile(property);
                }
                else // иначе "бросаем исключение" для информирования о некорректном формате файла
                {
                    throw new Exception("Incorrect image format!!!");
                }
            }
            else // иначе "бросаем исключение" для информирования об отсутствии файла по указанному пути
            {
                throw new Exception("File not exist!!!");
            }
        }
        #endregion
    }

    public class PartnerModel : CompanyModel
    {
        #region "Declarations"
        // ID партнёра
        private int id;
        // номер дисконтной карты
        private string discountCard;
        // электронная почта
        private string e_mail;
        // ID группы партнёра
        private int groupId;
        // скидка по группе партнёра
        private int partnerGroupDiscount;
        // статус партнёра (напр., удалён, часто используется и т.п.)
        private int status;
        #endregion

        #region "Constructor"
        /// <summary>
        /// Заполняет экземпляр пустыми данными
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>15.01.2021</date>
        public PartnerModel() : base()
        {
            ID = 0;
            DiscountCard = string.Empty;
            E_mail = string.Empty;
            GroupID = 0;
            Status = (int)Statuses.Available;
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// ID партнёра
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>15.01.2021</date>
        public int ID
        {
            get => id;
            set => id = value;
        }

        /// <summary>
        /// Номер дисконтной карты
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>15.01.2021</date>
        public string DiscountCard
        {
            get => discountCard;
            set => discountCard = value;
        }

        /// <summary>
        /// Электронная почта
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>15.01.2021</date>
        public string E_mail
        {
            get => e_mail;
            set => e_mail = value;
        }

        /// <summary>
        /// ID группы партнёра
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>15.01.2021</date>
        public int GroupID
        {
            get => groupId;
            set => groupId = value;
        }

        /// <summary>
        /// Скидка по группе партнёра
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>01.02.2021</date>
        public int PartnerGroupDiscount
        {
            get => partnerGroupDiscount;
            set => partnerGroupDiscount = value;
        }

        /// <summary>
        /// Статус партнёра (напр., удалён, часто используется и т.п.)
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>15.01.2021</date>
        public int Status
        {
            get => status;
            set => status = value;
        }
        #endregion

        public static explicit operator PDFCreator.Models.CompanyModel(PartnerModel partnerModel)
        {
            PDFCreator.Models.CompanyModel companyModel = new PDFCreator.Models.CompanyModel();
            companyModel.Name = partnerModel.Company;
            companyModel.Address = partnerModel.Address;
            companyModel.Principal = partnerModel.Principal;
            companyModel.TaxNumber = partnerModel.TaxNumber;
            companyModel.Phone = partnerModel.Phone;
            companyModel.VATNumber = partnerModel.VATNumber;

            return companyModel;
        }
    }

    public class CompanyModel
    {
        #region "Declarations"
        // название фирмы
        private string company;
        // материально-ответственное лицо
        private string principal;
        // город размещения/регистрации фирмы
        private string city;
        // адрес, по которому размещена/зарегистрирована фирма
        private string address;
        // телефон
        private string phone;
        // налоговый номер
        private string taxNumber;
        // номер НДС
        private string vatNumber;
        // номер банковского счета
        private string iBAN;
        #endregion

        #region "Constructor"
        /// <summary>
        /// Заполняет экземпляр пустыми данными
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>15.01.2021</date>
        public CompanyModel()
        {
            Company = string.Empty;
            Principal = string.Empty;
            City = string.Empty;
            Address = string.Empty;
            Phone = string.Empty;
            TaxNumber = string.Empty;
            VATNumber = string.Empty;
            IBAN = string.Empty;
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// Название фирмы
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>15.01.2021</date>
        public string Company
        {
            get => company;
            set => company = value;
        }

        /// <summary>
        /// Материально-ответственное лицо
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>15.01.2021</date>
        public string Principal
        {
            get => principal;
            set => principal = value;
        }

        /// <summary>
        /// Город размещения/регистрации фирмы
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>15.01.2021</date>
        public string City
        {
            get => city;
            set => city = value;
        }

        /// <summary>
        /// Адрес, по которому размещена/зарегистрирована фирма
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>15.01.2021</date>
        public string Address
        {
            get => address;
            set => address = value;
        }

        /// <summary>
        /// Телефон
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>15.01.2021</date>
        public string Phone
        {
            get => phone;
            set => phone = value;
        }

        /// <summary>
        /// Налоговый номер
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>15.01.2021</date>
        public string TaxNumber
        {
            get => taxNumber;
            set => taxNumber = value;
        }

        /// <summary>
        /// Номер НДС
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>15.01.2021</date>
        public string VATNumber
        {
            get => vatNumber;
            set => vatNumber = value;
        }

        /// <summary>
        /// Номер банковского счета
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>12.02.2021</date>
        public string IBAN
        {
            get => iBAN;
            set => iBAN = value;
        }
        #endregion
    }

    public enum DocumentType
    {
        Invoice = 0, // фактура
        DebitNote = 1, // дебитно известие
        CreditNote = 2, // кредитно известие
        ProformInvoice = 106, // проформа фактура
        Receipt = 1000, // стокова расписка
        Report = 1001 // отчет
    }

    public class PrintParametersModel
    {
        #region "Declarations"
        // перечень подключенных к данному ПК принтеров
        private List<string> installedPrinters;
        // выбранный пользователем принтер
        private string selectedPrinter;
        // перечень форматов бумаги
        private List<ComboBoxObject> pageFormats;
        // выбранный пользователем формат бумаги
        private ComboBoxObject selectedPageFormat;
        // перечень вариантов ориентации бумаги
        private List<ComboBoxObject> pageOrientations;
        // выбранный пользователем вариант ориентации бумаги
        private ComboBoxObject selectedPageOrientation;
        // оступ слева от края листа 
        private double leftMargin;
        // оступ сверху от края листа
        private double topMargin;
        // оступ справа от края листа
        private double rightMargin;
        // оступ снизу от края листа
        private double bottomMargin;
        // кол-во копий документа
        private int countCopies;
        #endregion

        #region "Constructor"
        /// <summary>
        /// Создаёт экземпляр класса и заполняет свойства данными, полученными от PrintService
        /// </summary>
        /// <param name="printService">Объект класса, обеспечивающего печать документов</param>
        /// <developer>Сергей Рознюк</developer>
        /// <date>02.04.2021</date>
        public PrintParametersModel()
        {
            installedPrinters = new List<string>();
            installedPrinters.Add("Printer A");
            installedPrinters.Add("Printer B");
            installedPrinters.Add("Printer C");
            SelectedPrinter = "Printer A";
            //foreach (string printerName in printService.InstalledPrinters)
            //{
            //    InstalledPrinters.Add(printerName);
            //}

            //if (InstalledPrinters.Count > 0)
            //{
            //    SelectedPrinter = installedPrinters[0];
            //}

            ComboBoxObject pageFormat;
            pageFormat = new ComboBoxObject("A3", "A3");
            PageFormats.Add(pageFormat);
            pageFormat = new ComboBoxObject("A4", "A4");
            PageFormats.Add(pageFormat);
            pageFormat = new ComboBoxObject("A5", "A5");
            PageFormats.Add(pageFormat);
            pageFormat = new ComboBoxObject("B5", "B5");
            PageFormats.Add(pageFormat);
            //foreach (PageFormat format in Enum.GetValues(typeof(PageFormat)))
            //{
            //    pageFormat = new ComboBoxObject(format.ToString(), format);
            //    PageFormats.Add(pageFormat);

            //    if (format == printService.PageFormat)
            //    {
            //        SelectedPageFormat = pageFormat;
            //    }
            //}

            ComboBoxObject pageOrientation;
            pageOrientation = new ComboBoxObject("Portrait", "Portrait");
            PageOrientations.Add(pageOrientation);
            pageOrientation = new ComboBoxObject("Landscape", "Landscape");
            PageOrientations.Add(pageOrientation);
            //foreach (PageOrientation orientation in Enum.GetValues(typeof(PageOrientation)))
            //{
            //    pageOrientation = new ComboBoxObject("string" + orientation.ToString(), orientation);
            //    PageOrientations.Add(pageOrientation);

            //    if(orientation == printService.PageOrientation)
            //    {
            //        SelectedPageOrientation = pageOrientation;
            //    }
            //}

            LeftMargin = 0.5;
            TopMargin = 0;
            RightMargin = 0;
            BottomMargin = 0;

            CountCopies = 1; // printService.CountCopies;
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// Перечень подключенных к данному ПК принтеров
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>02.04.2021</date>
        public List<string> InstalledPrinters
        {
            get => installedPrinters == null ? installedPrinters = new List<string>() : installedPrinters;
            set => installedPrinters = value;
        }

        /// <summary>
        /// Выбранный пользователем принтер
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>02.04.2021</date>
        public string SelectedPrinter
        {
            get => selectedPrinter;
            set => selectedPrinter = value;
        }

        /// <summary>
        /// Перечень доступных форматов бумаги
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>02.04.2021</date>
        public List<ComboBoxObject> PageFormats
        {
            get => pageFormats == null ? pageFormats = new List<ComboBoxObject>() : pageFormats;
            set => pageFormats = value;
        }

        /// <summary>
        /// Выбранный пользователем формат бумаги
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>02.04.2021</date>
        public ComboBoxObject SelectedPageFormat
        {
            get => selectedPageFormat == null ? selectedPageFormat = new ComboBoxObject() : selectedPageFormat;
            set => selectedPageFormat = value;
        }

        /// <summary>
        /// Перечень вариантов ориентации бумаги
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>02.04.2021</date>
        public List<ComboBoxObject> PageOrientations
        {
            get => pageOrientations == null ? pageOrientations = new List<ComboBoxObject>() : pageOrientations;
            set => pageOrientations = value;
        }

        /// <summary>
        /// Выбранный пользователем вариант ориентации бумаги
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>02.04.2021</date>
        public ComboBoxObject SelectedPageOrientation
        {
            get => selectedPageOrientation == null ? selectedPageOrientation = new ComboBoxObject() : selectedPageOrientation;
            set => selectedPageOrientation = value;
        }

        /// <summary>
        /// Оступ слева от края листа 
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>02.04.2021</date>
        public double LeftMargin
        {
            get => leftMargin;
            set => leftMargin = value;
        }

        /// <summary>
        /// Оступ сверху от края листа 
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>02.04.2021</date>
        public double TopMargin
        {
            get => topMargin;
            set => topMargin = value;
        }

        /// <summary>
        /// Оступ справа от края листа 
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>02.04.2021</date>
        public double RightMargin
        {
            get => rightMargin;
            set => rightMargin = value;
        }

        /// <summary>
        /// Оступ снизу от края листа 
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>02.04.2021</date>
        public double BottomMargin
        {
            get => bottomMargin;
            set => bottomMargin = value;
        }

        /// <summary>
        /// Кол-во печатаемых копий документа
        /// </summary>
        /// <developer>Сергей Рознюк</developer>
        /// <date>02.04.2021</date>
        public int CountCopies
        {
            get => countCopies;
            set => countCopies = value;
        }
        #endregion
    }
    public enum Statuses
    {
        Hidden = -1, // скрытый
        Available // доступный
    }

    public class ComboBoxObject
    {
        #region "Constructors"
        public ComboBoxObject()
        {
            Text = string.Empty;
            Value = null;
        }

        public ComboBoxObject(string text, object value)
        {
            Text = text;
            Value = value;
        }

        #endregion

        #region "Properties"
        /// <summary>
        /// Текст, который будет отображаться в ComboBox-е
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Дополнительный параметр (параметры), записываемые в элемент ComboBox-а
        /// </summary>
        public object Value { get; set; }
        #endregion

        #region "Overrides methods"
        /// <summary>
        /// Значение, отображаемое пользователю на экране
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
        }

      
        #endregion
    }

    public enum DocumentVersionPrinting
    {
        Original,
        Copy,
        OriginalAndCopy,
        OriginalAndTwoCopies
    }

    public enum OperationType
    {
        Sale = 2, // продажа
        Refund = 34 // возврат товара от клиента
    }

    public enum PaymentTypes
    {
        Undefined = 0,  // неопределённый тип
        Cash = 1,       // наличными
        Bank = 2,       // по счёту
        Card = 3,       // дебитная/кредитная карта
        Voucher = 4,    // ваучер
        Rewards = 5,    // электронные точки
        Other1 = 6,     // другой тип оплаты 1
        Other2 = 7,     // другой тип оплаты 2
        Other3 = 8,     // другой тип оплаты 3
        Other4 = 9      // другой тип оплаты 4
    }

}

