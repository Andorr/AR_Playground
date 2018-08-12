from rest_framework import serializers

from .models import Page, Document

import base64

import logging


def isBase64(s):
    try:
        return base64.b64encode(base64.b64decode(s)) == s
    except Exception:
        return False


class BinaryField(serializers.Field):
    """
    Color objects are serialized into 'rgb(#, #, #)' notation.
    """
    def to_representation(self, obj):
        return bytes(obj)

    def to_internal_value(self, data):
        return '#YOLO'


class PageSerializer(serializers.ModelSerializer):

    data = BinaryField()

    class Meta:
        model = Page
        fields = ('number', 'data')


class DocumentSerializer(serializers.ModelSerializer):

    pages = PageSerializer(many=True, read_only=True)

    class Meta:
        model = Document
        depth = 1
        fields = ('name', 'pages')