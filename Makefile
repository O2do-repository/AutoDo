init: requirements.txt
	pip install -r requirements.txt

test: 
	pytest

run:
	python Scraper.py

clean:
	rm -rf __pycache__
	rm -rf .pytest_cache

.PHONY: init test