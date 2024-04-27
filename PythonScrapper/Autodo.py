from Scraper import InfrabelPDFScraper
import os

infrabel_pdf_folder_path = "tests/test_files"


def main():
    for file in os.listdir(infrabel_pdf_folder_path):
        scraper = InfrabelPDFScraper()
        opportunity = scraper.scrapePDF(os.path.join(infrabel_pdf_folder_path, file))
        opportunity.save()


if __name__ == "__main__":
    main()
