"""Unit tests for Sportradar provider."""

# Skip if provider not implemented
import pytest

try:
    from src.api_providers.sportradar import SportradarAPIProvider
    SPORTRADAR_AVAILABLE = True
except ImportError:
    SPORTRADAR_AVAILABLE = False
    SportradarAPIProvider = None


@pytest.mark.skipif(not SPORTRADAR_AVAILABLE, reason="Sportradar provider not implemented")
class TestSportradarProvider:
    """Unit tests for SportradarAPIProvider."""
    
    def test_init(self):
        """Test provider initialization."""
        provider = SportradarAPIProvider(api_key="test_key", enabled=True, priority=1)
        assert provider.api_key == "test_key"
        assert provider.enabled is True
        assert provider.priority == 1
    
    def test_get_provider_name(self):
        """Test provider name."""
        provider = SportradarAPIProvider(api_key="test")
        assert "sportradar" in provider.get_provider_name().lower()
    
    def test_normalize_response_happy_path(self):
        """Test normalizing valid API response."""
        provider = SportradarAPIProvider(api_key="test")

        raw_response = {
            "schedules": [
                {
                    "sport_event": {
                        "sport_event_context": {"sport": {"name": "Soccer"}},
                        "competitors": [
                            {"name": "Team A", "qualifier": "home"},
                            {"name": "Team B", "qualifier": "away"},
                        ],
                        "start_time": "2024-01-01T12:00:00Z",
                    },
                    "sport_event_status": {"status": "closed"},
                    "markets": [
                        {
                            "name": "Match Winner",
                            "outcomes": [{"name": "Team A", "odds": "2.0"}, {"name": "Team B", "odds": "2.5"}],
                        }
                    ],
                }
            ]
        }

        result = provider.normalize_response(raw_response)
        assert isinstance(result, list)

        # Should return list of events
        if result:
            assert isinstance(result[0], dict)

    def test_normalize_response_empty(self):
        """Test normalizing empty response."""
        provider = SportradarAPIProvider(api_key="test")
        result = provider.normalize_response({})
        assert isinstance(result, list)
        result = provider.normalize_response({"schedules": []})
        assert isinstance(result, list)
    
    def test_normalize_response_error_path(self):
        """Test normalizing invalid response."""
        provider = SportradarAPIProvider(api_key="test")

        # Invalid data types
        assert isinstance(provider.normalize_response(None), list)
        assert isinstance(provider.normalize_response([]), list)
        assert isinstance(provider.normalize_response("invalid"), list)
