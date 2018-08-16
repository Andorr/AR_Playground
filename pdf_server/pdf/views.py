from django.shortcuts import render

# Create your views here.

from rest_framework.response import Response
from rest_framework.decorators import api_view

from .serializers import DocumentSerializer
from .models import Document


@api_view(['GET'])
def hello(request):
    queryset = Document.objects.all()
    return Response(DocumentSerializer(queryset, many=True).data)