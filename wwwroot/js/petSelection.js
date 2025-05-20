// petSelection.js - Helpers for pet selection process

window.petSelectionHelpers = {
    selectPet: function(petId) {
        console.log('JS: Selecting pet ID:', petId);
        
        // Remove active class from all pet cards
        document.querySelectorAll('.pet-card').forEach(card => {
            card.classList.remove('border-primary', 'shadow');
        });
        
        // Add active class to the selected pet card
        const selectedCard = document.querySelector(`.pet-card[data-pet-id="${petId}"]`);
        if (selectedCard) {
            selectedCard.classList.add('border-primary', 'shadow');
            console.log('JS: Found and updated pet card');
        }
        
        // Update the Next button state
        const nextButton = document.querySelector('#nextButtonStep3');
        if (nextButton && petId > 0) {
            nextButton.removeAttribute('disabled');
            console.log('JS: Enabled Next button');
        }
        
        // Update the hidden pet ID field
        const hiddenPetIdField = document.querySelector('#hiddenPetId');
        if (hiddenPetIdField) {
            hiddenPetIdField.value = petId;
            console.log('JS: Updated hidden pet ID field to', petId);
        }
        
        return true;
    },
    
    initializePetSelection: function() {
        console.log('JS: Initializing pet selection');
        
        // Check if we're on step 3
        const step3Content = document.querySelector('.progress-bar[aria-valuenow="3"]');
        if (!step3Content) return;
        
        // Find the selected pet ID from the badges
        const selectedPetBadge = document.querySelector('.badge.bg-info');
        if (selectedPetBadge) {
            const text = selectedPetBadge.textContent;
            const match = text.match(/Selected Pet ID: (\d+)/);
            if (match && match[1] && parseInt(match[1]) > 0) {
                const petId = parseInt(match[1]);
                console.log('JS: Found pet ID in badge:', petId);
                this.selectPet(petId);
            }
        }
    }
};

// Initialize when the DOM is fully loaded
document.addEventListener('DOMContentLoaded', function() {
    console.log('DOM loaded, initializing pet selection');
    window.petSelectionHelpers.initializePetSelection();
}); 