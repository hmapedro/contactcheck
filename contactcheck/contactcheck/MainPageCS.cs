using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using friendcheck.CustomClasses;
using Microsoft.Maui.ApplicationModel.Communication;

namespace friendcheck
{
	public class MainPageCS : ContentPage
    {
        CollectionView collectionViewContacts;
        ObservableCollection<Contact> contacts;
        AbsoluteLayout absoluteLayout;

        public MainPageCS()
        {
            this.BackgroundColor = Colors.Black;
            absoluteLayout = new AbsoluteLayout
            {
                Margin = new Thickness(0)
            };
            Content = absoluteLayout;


            Label contactosLabel = new Label
            {
                Text = "CONTACTOS",
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 14,
                FontFamily = "futuracondensedmedium",
            };

            absoluteLayout.Add(contactosLabel);
            absoluteLayout.SetLayoutBounds(contactosLabel, new Rect(0, 0, 400, 30));

            GetContactNames();
            CreateContactsColletion();
            //InitializeComponent();
        }

        public async void GetContactNames()
        {
            IEnumerable<Contact> contacts_ = await Microsoft.Maui.ApplicationModel.Communication.Contacts.GetAllAsync();
            contacts = new ObservableCollection<Contact>(contacts_);
        }


        public void CreateContactsColletion()
        {

            Debug.Print("CreateContactsColletion");
            //COLLECTION Contacts
            collectionViewContacts = new CollectionView
            {
                SelectionMode = SelectionMode.Single,
                ItemsSource = contacts,
                ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical) { VerticalItemSpacing = 10, HorizontalItemSpacing = 5, },
                EmptyView = new ContentView
                {
                    Content = new Microsoft.Maui.Controls.StackLayout
                    {
                        Children =
                            {
                                new Label { Text = "Não tem Contactos associados.", HorizontalTextAlignment = TextAlignment.Start, TextColor = Colors.White, FontSize = 14 },
                            }
                    }
                }
            };

            collectionViewContacts.SelectionChanged += OnCollectionViewContactSelectionChanged;

            collectionViewContacts.ItemTemplate = new DataTemplate(() =>
            {

                AbsoluteLayout itemabsoluteLayout = new AbsoluteLayout
                {
                    HeightRequest = 30
                };

                Label nameLabel = new Label
                {
                    TextColor = Color.FromRgb(96, 182, 89),
                    HorizontalTextAlignment = TextAlignment.Center,
                    FontSize = 14,
                    FontFamily = "futuracondensedmedium",
                };
                nameLabel.SetBinding(Label.TextProperty, "DisplayName");

                itemabsoluteLayout.Add(nameLabel);
                itemabsoluteLayout.SetLayoutBounds(nameLabel, new Rect(0, 0, 100, 30));

                return itemabsoluteLayout;
            });

            absoluteLayout.Add(collectionViewContacts);
            absoluteLayout.SetLayoutBounds(collectionViewContacts, new Rect(0, 40, 400, 400));
        }

        public void OnCollectionViewContactSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if ((sender as CollectionView).SelectedItem != null)
            {
                Contact contact = (sender as CollectionView).SelectedItem as Contact;

                DisplayAlert("Olá", "olá " + contact.DisplayName, "Ok");

                sendSMSAsync(contact);
            }

        }

        public async Task sendSMSAsync(Contact contact)
        {
            Debug.Print("sendSMSAsync");
            if (Sms.Default.IsComposeSupported)
            {
                Debug.Print("sendSMSAsync1");
                string[] recipients = new[] { contact.Phones.ElementAt(0).ToString() };
                string text = "Hello " + contact.DisplayName;

                var message = new SmsMessage(text, recipients);

                await Sms.Default.ComposeAsync(message);
            }
            Debug.Print("sendSMSAsync2");
        }
    }
}

