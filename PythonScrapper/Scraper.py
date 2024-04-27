from pypdf import PdfReader
import os
import re
from datetime import datetime
from Model import Opportunity


def format_to_datetime(date_as_str: str) -> datetime.date:
    """Transforms a string formatted as dd-mm-yyyy into a Date

    Args:
        date_as_str (str): dd-mm-yyyy

    Returns:
        datetime.date: formatted date
    """
    return datetime.strptime(date_as_str, "%d-%m-%Y").date()


def textFromPDF(file_path) -> str:
    """Extract the entire text content from a PDF

    Args:
        file_path (LiteralString): The path to the pdf file

    Returns:
        str: The text exctracted
    """
    pdf_text = ""
    with open(file_path, "rb") as file:
        # Initialiser l'objet PDF reader
        pdf_reader = PdfReader(file)

        # Vérifier le nombre total de pages dans le PDF
        num_pages = len(pdf_reader.pages)

        # Parcourir chaque page et extraire le texte
        pdf_text = ""
        for page_num in range(num_pages):
            # Obtenir l'objet Page pour la page actuelle
            page = pdf_reader.pages[page_num]

            # Extraire le texte de la page
            page_text = page.extract_text()

            # Rajouter le contenu de la page au contenu global du pdf
            pdf_text += page_text

    return pdf_text


def extract(text: str, regex, transformation_method=lambda x: x):
    """
    This generic method is to extract the first group from a regex matching
    The result is normally a string but can be applied to a transformation function

    Args:
        text (str): The text in which we look for the pattern
        regex (_type_): The pattern.
        transformation_method (function, optional): The transformation method applied to the string. Defaults to lambdax:x.

    Returns:
        str | None | Other : Normally returns a string or the result of the transformation function.  When no match found, returns None.
    """
    # Utiliser une expression régulière pour trouver la date au format JJ-MM-AAAA
    match = re.search(regex, text)
    if match:
        # Extraire la chaîne de caractères représentant la date
        raw_extract = match.group(1)
        # Convertir la chaîne de caractères en objet de date
        transformed_extract = transformation_method(raw_extract)
        return transformed_extract
    else:
        return None


def remove_new_lines(text: str) -> str:
    """Remove any new line character from a string and replaces it by a space

    Args:
        text (str): The original text

    Returns:
        str: The cleansed text
    """
    return text.replace("\n", " ")


class InfrabelPDFScraper:
    """A scraper dedicated for the Infrabel PDF info"""

    def extract_id(self, text: str):
        return extract(text, r"Marché \| Opdracht: (\d{5})")

    def extract_deadline(self, text: str):
        return extract(
            text,
            r"Fin de publication \| Einde publicatie: (\d{2}-\d{2}-\d{4})",
            format_to_datetime,
        )

    def extract_name(self, text: str):
        return extract(
            text,
            r"Titre: ((\w| )+)",
        )

    def extract_description(self, text: str):
        return remove_new_lines(extract(text, r"Description de la tâche: \"([^\"]+)\""))

    def extract_publication_date(self, text: str):
        return extract(
            text, r"\d{5} (\d{2}-\d{2}-\d{4}) \d{2}:\d{2}:\d{2}", format_to_datetime
        )

    def extract_experience(self, text: str):
        return extract(text, r"Expérience \| Ervaring: ((.)+)\n")

    def extract_skills(self, text: str):
        return remove_new_lines(extract(text, r"Technical skills: \"(([^\"])+)\""))

    def extract_place_to_work(self, text: str):
        return extract(text, r"Lieu de travail \| Plaats tewerkstelling ((.)+)\n")

    def scrapeText(self, text: str) -> Opportunity:
        id = self.extract_id(text)
        name = self.extract_name(text)
        published_on = self.extract_publication_date(text)
        deadline = self.extract_deadline(text)
        place_to_work = self.extract_place_to_work(text)
        description = self.extract_description(text)
        client = "Infrabel"
        experience = self.extract_experience(text)
        skills = self.extract_skills(text)
        opportunity = Opportunity(
            id=id,
            name=name,
            client=client,
            publication_date=published_on,
            submission_deadline=deadline,
            raw_content=text,
            place_to_work=place_to_work,
            description=description,
            experience=experience,
            skills=skills,
        )

        return opportunity

    def scrapePDF(self, file) -> Opportunity:
        """Takes the path to a PDF file and returns a formatted opportunity

        Args:
            file (LiteralString): the path to the pdf file

        Returns:
            Opportunity: formatted opportunity object
        """
        pdf_text = textFromPDF(file)
        opportunity = self.scrapeText(pdf_text)
        return opportunity


def main():
    # Chemin vers le dossier contenant le fichier PDF
    folder_path = "tests/test_files"

    # Nom du fichier PDF à lire
    file_name = "Offerteaanvraag 00691.pdf"

    # Chemin complet vers le fichier PDF
    file_path = os.path.join(folder_path, file_name)

    scraper = InfrabelPDFScraper()
    opportunity = scraper.scrapePDF(file_path)

    opportunity.save()


if "__main__":
    main()
