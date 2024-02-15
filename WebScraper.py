from selenium import webdriver
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.by import By
from dotenv import load_dotenv
import os

load_dotenv()


class ConnectingExpertisePlatform:
    def __init__(self, browser=webdriver.Chrome()):
        self.url = "https://customer.connecting-expertise.com"
        self.browser = browser

    def __enter__(self):
        self.browser = webdriver.Chrome()
        return self

    def __exit__(self):
        self.browser.quit()

    def fill_in_connection_form(self, username: str, password: str) -> None:
        self.browser.get(self.url)
        self.browser.implicitly_wait(0.5)

        username_field = self.browser.find_element(By.NAME, "email")
        username_field.send_keys(username)

        password_field = self.browser.find_element(By.ID, "passWordField")
        password_field.send_keys(password)

        password_field.send_keys(Keys.ENTER)

        # Attendre que la page se charge après l'authentification
        self.browser.implicitly_wait(10)

    def navigate_to_request_page(self) -> None:
        # Naviger vers la liste des offres
        demand_tab = self.browser.find_element(By.ID, "menu-seller-rfqs")
        demand_link = demand_tab.find_element(By.TAG_NAME, "a")
        demand_link.click()
        self.browser.implicitly_wait(10)


username = os.getenv("CONNECTING_EXPERTISE_USERNAME")
password = os.getenv("CONNECTING_EXPERTISE_PASSWORD")
with ConnectingExpertisePlatform() as website:
    website.fill_in_connection_form(username, password)
    website.navigate_to_request_page()
