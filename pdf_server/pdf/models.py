from django.db import models

# Create your models here.


class Document(models.Model):

    name = models.CharField(max_length=200)

    @property
    def pages(self):
        return Page.objects.filter(document=self)


class Page(models.Model):
    number = models.IntegerField()

    document = models.ForeignKey(Document, on_delete=models.CASCADE)

    data = models.BinaryField()
