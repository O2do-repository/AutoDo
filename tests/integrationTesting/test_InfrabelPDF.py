from Scraper import InfrabelPDFScraper
import datetime
import os


class TestInfrabelScraptingClass:
    # Chemin vers le dossier contenant le fichier PDF
    folder_path = "tests/test_files"
    # Nom du fichier PDF à lire
    file_name = "Offerteaanvraag 00691.pdf"
    # Chemin complet vers le fichier PDF
    file_path = os.path.join(folder_path, file_name)

    def test_infrabel_pdf_scraping(self):
        scraper = InfrabelPDFScraper()
        opportunity = scraper.scrapePDF(self.file_path)

        assert opportunity.id == "00691"
        assert opportunity.name == "Business Analyst Trieerheuvel"
        assert opportunity.client == "Infrabel"
        assert opportunity.publication_date == datetime.date(2024, 2, 2)
        assert opportunity.submission_deadline == datetime.date(2024, 2, 26)
        assert type(opportunity.raw_content) == str
        assert opportunity.place_to_work == "Rue des deux gares 82, 1070 Bruxelles"
        assert type(opportunity.description) == str
        assert opportunity.description.startswith(
            "Contexte Business et but Le Port d'Anvers-Bruges et Infrabel collaborent et s"
        )
        assert opportunity.description.endswith(
            "prenantes internes pour obtenir des informations et partager des idées."
        )
        assert opportunity.experience == "05 - 10 jaar (Senior)"
        assert type(opportunity.skills) == str
        assert opportunity.skills.startswith(
            "(M) = Mandatory skill / une expérience professionnelle avec le"
        )
        assert opportunity.skills.endswith(
            "la synthèse des exigences non fonctionnelles  • (M) Une bonne connaissance du néerlandais est requise • Lire, comprendre, parler et écrire en français et anglais est également recommandé"
        )
