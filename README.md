# AutoDo

## Getting Started

### Build

Just make sure you set up correctly your environment

* Python3 (>3.11)
* run `pip3 install -r requirements.txt`

to update the requirements.txt: `pip3 freeze > requirements.txt`

### Test

to be completed

### Run

run `python3 Scraper.py`

## Business Objective

This projet contains automations scripts to automatize administrative tasks within O2Do.

### Scraping opportunities automatically

The first goal is to scrape opportunities for new assignment automatically so we don't have to read them all by logging into multiple platforms and scrolling to get only the newest and interesting offers.

In the future, it will allow

* automatic matching with profiles
* keeping performance statistics on our candidates
* (very future) anticipate needs from clients?

#### Infrabel

Infrabel sends emails with PDF files attached to it containing all the information about the opportunity.
With PowerAutomate, we can place any PDF received into a dedicated folder on Sharepoint.
This workflow can then run and parse the PDF in order to persist the info.
