# Gizindir — Windows Forms Swipe Chat Uygulaması

---

## Proje Hakkında

**Gizindir**, kullanıcıların profil kartlarını sağa veya sola kaydırarak (swipe) beğendiği veya beğenmediği kişileri seçtiği ve karşılıklı beğenilerde anlık sohbet başlatabildiği, Windows Forms tabanlı yenilikçi bir arkadaşlık uygulamasıdır.

Kalıpların dışına çıkarak, kullanıcı deneyimini basit, hızlı ve etkileyici bir şekilde yeniden tanımlayan, yepyeni bir soluk sunar.

---

## Kullanılan Mimari: MVVM Yaklaşımı

Windows Forms projelerinde doğal olarak desteklenmeyen MVVM yaklaşımı, **Gizindir** uygulamasında temiz, modüler ve sürdürülebilir kod yapısı için benzer prensiplerle uygulanmıştır:

- **Model:** Temel veri yapıları ve iş mantığı  
- **View:** Kullanıcı arayüzü, sadece gösterim ve kullanıcı etkileşimlerini ViewModel’a aktarır  
- **ViewModel:** Model ile View arasında köprü, iş mantığını yönetir, UI’a veri sağlar ve kullanıcı girdilerini işler

Bu yapı sayesinde Windows Forms uygulamasında bile modern, yönetilebilir ve test edilebilir bir mimari yakalanmıştır.

---

## Uygulama Akışı

1. **Kullanıcı Kaydı ve Profil Oluşturma**  
2. **Kart Kaydırma Ekranı** – Hızlı ve sezgisel swipe hareketleriyle kullanıcı kartlarını beğenme ya da geçme  
3. **Eşleşme ve Anlık Sohbet** – Karşılıklı beğenilerde gerçek zamanlı mesajlaşma başlar  
4. **Tema Yönetimi** – Karanlık mod ve açık mod desteği ile kullanıcı deneyimi kişiselleştirilebilir

---

## Teknik Detaylar

- **Platform:** Windows Forms (.NET Framework)  
- **Mimari:** MVVM prensiplerine uygun katmanlı yapı  
- **Veri Yönetimi:** Uygulama içi modeller ve opsiyonel harici veri tabanı desteği  
- **Kart Kaydırma:** Özel kontrol ve mouse hareketleri ile swipe efekti  
- **Anlık Mesajlaşma:** Gerçek zamanlı iletişim için uygun altyapı (isteğe bağlı)

---

## Proje Yapısı

/Gizindir
├── Models/ # Veri modelleri (User, Match, Message)
/Views/ # UI ekranları
/ViewModels/ # İş mantığı
/Services/ # Veri ve iletişim servisleri
/Themes/ # Açık/Karanlık mod temaları
/Utils/ # Yardımcı araçlar

yaml
Kopyala
Düzenle

---

## Karanlık Mod & Açık Mod

Kullanıcıların tercihlerine göre arayüz teması dinamik olarak değişir. Renkler, butonlar ve arka plan Windows Forms bileşenleri üzerinde efektif şekilde güncellenir.

---

## Nasıl Çalışır?

- Uygulama başlar, kullanıcı kayıt ekranı açılır  
- Kullanıcı profil bilgilerini girer  
- Kart kaydırma ekranında diğer kullanıcılar listelenir  
- Beğenilen kişilerle eşleşme sağlanır, anlık sohbet başlatılır  
- Tema tercihi istenildiği zaman değiştirilebilir

---

## Kurulum ve Çalıştırma

1. Projeyi Visual Studio’da açın  
2. Gerekli bağımlılıkları yükleyin  
3. Derleyip çalıştırın

---

## Katkı & Destek

Katkıda bulunmak isterseniz, lütfen kod standartlarına ve MVVM prensiplerine dikkat ederek pull request gönderin.

---

## İletişim

Sorularınız için: example@email.com

---

## Lisans

MIT Lisansı ile lisanslanmıştır.

---

# Yeni Bir Solukla Tanışın!

Gizindir, klasik kalıpları kıran, kullanıcı deneyimini kolaylaştıran ve daha samimi bir iletişim ortamı sunan yepyeni bir arkadaşlık uygulamasıdır.

---
