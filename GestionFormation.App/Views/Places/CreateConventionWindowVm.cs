using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GestionFormation.App.Core;
using GestionFormation.App.Views.EditableLists;
using GestionFormation.Applications.Contacts;
using GestionFormation.Applications.Conventions;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Contacts.Queries;
using GestionFormation.CoreDomain.Conventions;

namespace GestionFormation.App.Views.Places
{
    public class CreateConventionWindowVm : PopupWindowVm
    {
        private readonly IApplicationService _applicationService;
        private readonly IContactQueries _contactQueries;
        private readonly List<PlaceItem> _selectedPlaces;
        private readonly IComputerService _computerService;
        private ObservableCollection<PlaceItem> _places;
        private ObservableCollection<ContactItem> _contacts;
        private ContactItem _selectedContact;
        private TypeConvention _typeConvention;

        public CreateConventionWindowVm(IApplicationService applicationService, IContactQueries contactQueries, List<PlaceItem> selectedPlaces, IComputerService computerService)
        {            
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _contactQueries = contactQueries ?? throw new ArgumentNullException(nameof(contactQueries));
            _selectedPlaces = selectedPlaces ?? throw new ArgumentNullException(nameof(selectedPlaces));
            _computerService = computerService ?? throw new ArgumentNullException(nameof(computerService));
            AddContactCommand = new RelayCommandAsync(ExecuteAddContactAsync);
            UnknowTypeConventionCommand = new RelayCommand(ExecuteUnknowTypeConvention);

            SetValiderCommandCanExecute(() => SelectedContact != null);
        }

        public override async Task Init()
        {
            Places = new ObservableCollection<PlaceItem>(_selectedPlaces);
            await InitContacts(null);
        }

        public TypeConvention TypeConvention
        {
            get => _typeConvention;
            set { Set(() => TypeConvention, ref _typeConvention, value); }
        }

        public RelayCommand UnknowTypeConventionCommand { get; }
        private void ExecuteUnknowTypeConvention()
        {
            _computerService.OpenTypeConventionMail();
        }

        public ObservableCollection<PlaceItem> Places
        {
            get => _places;
            set { Set(() => Places, ref _places, value); }
        }

        public ObservableCollection<ContactItem> Contacts
        {
            get => _contacts;
            set { Set(()=>Contacts, ref _contacts, value); }
        }

        public RelayCommandAsync AddContactCommand { get; }
        private async Task ExecuteAddContactAsync()
        {
            var vm = await _applicationService.OpenPopup<CreateItemVm>("Cr�er un contact", new EditableContact());
            if (vm.IsValidated)
            {
                await HandleMessageBoxError.ExecuteAsync(async () =>
                {
                    var item = vm.Item as EditableContact;
                    var newItem = await Task.Run(() => _applicationService.Command<CreateContact>().Execute(item.Nom, item.Prenom, item.Email, item.Telephone));
                    await InitContacts(newItem.AggregateId);
                });
            }
        }

        private async Task InitContacts(Guid? selectedFormationId)
        {
            var contactsTask = await Task.Run(() => _contactQueries.GetAll().Select(a => new ContactItem(a)));
            Contacts = new ObservableCollection<ContactItem>(contactsTask);
            SelectedContact = Contacts.FirstOrDefault(a => a.Id == selectedFormationId);
        }

        public ContactItem SelectedContact
        {
            get => _selectedContact;
            set
            {
                Set(()=>SelectedContact, ref _selectedContact, value);
                ValiderCommand.RaiseCanExecuteChanged();
            }
        }

        protected override async Task ExecuteValiderAsync()
        {
            await HandleMessageBoxError.ExecuteAsync(async () => {
                await Task.Run(() => _applicationService.Command<CreateConvention>().Execute(SelectedContact.Id, Places.Select(a => a.PlaceId), TypeConvention));
                await base.ExecuteValiderAsync();
            });
        }
    }

    public class ContactItem
    {
        public ContactItem(IContactResult contactResult)
        {
            Id = contactResult.Id;
            Nom = contactResult.Nom;
            Prenom = contactResult.Prenom;
            Telephone = contactResult.Telephone;
            Email = contactResult.Email;
        }
        public Guid Id { get; }
        public string Nom { get; }
        public string Prenom { get; }
        public string Telephone { get; }
        public string Email { get; }

        public override string ToString()
        {
            return Nom + " " + Prenom;
        }
    }
}