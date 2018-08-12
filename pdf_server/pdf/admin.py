from django.contrib import admin

from django import forms
from django.contrib import admin

from .models import Document, Page


from pdf2image import convert_from_bytes

import base64
from io import BytesIO


class DocumentForm(forms.ModelForm):

    pdf = forms.FileField(required=False)

    def clean(self, *args, **kwargs):
        self.cleaned_data = super().clean()

        if self.cleaned_data['pdf']:
            images = convert_from_bytes(
                self.cleaned_data['pdf'].read()
            )
            for page_number, image in enumerate(images):
                buffered = BytesIO()
                image.save(buffered, format="JPEG")
                page = Page(number=page_number, document=self.instance, data=base64.b64encode(buffered.getvalue()))
                page.save()

        return self.cleaned_data

    class Meta:
        model = Document
        fields = ('name', )


class DocumentAdmin(admin.ModelAdmin):

    form = DocumentForm


class PageForm(forms.ModelForm):

    class Meta:
        model = Page
        fields = ('document', 'number')


class PageAdmin(admin.ModelAdmin):

    form = PageForm


admin.site.register(Document, DocumentAdmin)

admin.site.register(Page, PageAdmin)
