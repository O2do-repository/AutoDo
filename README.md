# AutoDo

## Getting Started

Just make sure you set up correctly your environment.

* Python3 (>3.11)

Then use the `Makefile`

### Build

* run `make init`

to update the requirements.txt: `pip3 freeze > requirements.txt`

### Test

run `make test`

### Run

run `make run`

## Business Objective

This project contains automation scripts to automatize administrative tasks within O2Do.

### Scraping opportunities automatically

The first goal is to scrape opportunities for a new assignment automatically so we don't have to read them all by logging into multiple platforms and scrolling to get only the newest and interesting offers.

In the future, it will allow

* automatic matching with profiles
* keeping performance statistics on our candidates
* (very future) anticipate needs from clients?

#### Infrabel

Infrabel sends emails with PDF files attached to it containing all the information about the opportunity.
With PowerAutomate, we can place any PDF received into a dedicated folder on Sharepoint.
This workflow can then run and parse the PDF in order to save the info.
