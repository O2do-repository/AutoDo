from dataclasses import dataclass
from datetime import datetime


@dataclass
class Opportunity:
    id: str
    name: str
    client: str
    publication_date: datetime
    submission_deadline: datetime
    raw_content: str
    place_to_work: str = "not provided"
    description: str = "not provided"
    experience: str = "not provided"
    skills: str = "not provided"

    def save(self):
        print(self)
