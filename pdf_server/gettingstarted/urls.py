from django.conf.urls import include, url
from django.urls import path

from django.contrib import admin

from pdf import views

admin.autodiscover()

urlpatterns = [
    path('', views.hello, name='hello'),
    path('admin/', admin.site.urls),
]
