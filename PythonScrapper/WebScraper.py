from selenium import webdriver
from selenium.webdriver.support.ui import Select
from selenium.common.exceptions import NoSuchElementException
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.by import By
from dotenv import load_dotenv
import os
from Model import Opportunity

load_dotenv()


class ConnectingExpertisePlatform:
    def __init__(self, browser=webdriver.Chrome()):
        self.url = "https://customer.connecting-expertise.com"
        self.browser = browser
        self.browser.implicitly_wait(5)

    def __enter__(self):
        self.browser = webdriver.Chrome()
        return self

    def __exit__(self, exc_type, exc_value, exc_tb):
        self.logout()
        self.browser.quit()

    def login(self, username: str, password: str) -> None:
        self.browser.get(self.url)

        username_field = self.browser.find_element(By.NAME, "email")
        username_field.send_keys(username)

        password_field = self.browser.find_element(By.ID, "passWordField")
        password_field.send_keys(password)

        password_field.send_keys(Keys.ENTER)

    def navigate_to_request_page(self) -> None:
        # Naviger vers la liste des offres
        demand_tab = self.browser.find_element(By.ID, "menu-seller-rfqs")
        demand_link = demand_tab.find_element(By.TAG_NAME, "a")
        demand_link.click()
        self.browser.implicitly_wait(5)
        select = Select(
            self.browser.find_element(By.NAME, "tbl_seller_requests_length")
        )
        select.select_by_value("100")

    def open_offer_detailed_page(self, offer_id: str) -> bool:
        self.browser.implicitly_wait(10)
        next_button = self.browser.find_element(By.CLASS_NAME, "next")
        try:
            self.browser.implicitly_wait(5)
            offer = self.browser.find_element(By.CLASS_NAME, offer_id)
            offer.click()
            return True
        except:
            # Si on est sur la dernière page, on arrête et on ne retourne rien
            if "disabled" in next_button.get_attribute("class"):
                return False
            # Sinon on clique sur page suivante et on recommence à chercher l'offre
            next_button.click()
            self.open_offer_detailed_page(offer_id)

    def get_content(self) -> str:
        return self.browser.find_element(By.CLASS_NAME, "content").text

    def logout(self) -> None:
        self.browser.find_element(By.CLASS_NAME, "logged-in-user").click()
        self.browser.find_element(By.ID, "user-menu-logout").click()


username = os.getenv("CONNECTING_EXPERTISE_USERNAME")
password = os.getenv("CONNECTING_EXPERTISE_PASSWORD")


class ConnectiveExpertiseScraper:
    def __init__(self, username, password, webdriver=ConnectingExpertisePlatform()):
        self.username = username
        self.password = password
        self.webdriver = webdriver

    def scrape_opportunities(self, opportunity_ids: list[str]) -> list[Opportunity]:
        opportunities = []
        with self.webdriver as website:
            website.login(username, password)
            website.navigate_to_request_page()
            for id in opportunity_ids:
                if website.open_offer_detailed_page(id):
                    opportunity = Opportunity(
                        id=id,
                        name=website.browser.find_element(By.ID, "rfqTitle")
                        .find_element(By.TAG_NAME, "div")
                        .text,
                        client="to be filled",
                        publication_date="to be filled",
                        submission_deadline=website.browser.find_element(
                            By.ID, "latestReactionDate"
                        )
                        .find_element(By.TAG_NAME, "h3")
                        .text,
                        raw_content=website.get_content(),
                        place_to_work=website.browser.find_element(By.ID, "location")
                        .find_element(By.TAG_NAME, "h3")
                        .text,
                        description=website.browser.find_element(
                            By.ID, "rfqDescription"
                        )
                        .find_element(By.TAG_NAME, "div")
                        .text,
                        # experience= ,
                        # skills= ,
                        url=website.browser.current_url,
                    )
                    opportunity.save()
                    opportunities.append(opportunity)
                website.navigate_to_request_page()

        return opportunities


def main():
    scraper = ConnectiveExpertiseScraper(username, password)
    scraper.scrape_opportunities(["FLU003171", "DOM000536"])


if "__main__":
    main()
